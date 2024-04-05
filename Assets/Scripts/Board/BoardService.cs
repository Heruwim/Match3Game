using StaticData;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CellFactory))]
public class BoardService : MonoBehaviour
{
    [SerializeField] private Sprite[] _cellSprites;
    [SerializeField] private ParticleSystem _matchFxPrefab;
    [SerializeField] private ScoreService _scoreService;

    private CellData[,] _boards;
    private CellFactory _cellFactory;
    private MatchMachine _matchMachine;
    private CellMover _cellMover;
    private readonly List<Cell> _updatingCells = new();
    private readonly List<Cell> _deadCells = new();
    private readonly List<CellFlip> _flippedCells = new();
    private readonly int[] _fillingCellsCountByColumn = new int[Config.BoardWidth];
    private readonly List<ParticleSystem> _matchFxs = new List<ParticleSystem>();

    public ArrayLayout BoartLayout;

    public Sprite[] CellSprites => _cellSprites;

    private void Awake()
    {
        _cellFactory = GetComponent<CellFactory>();
        _matchMachine = new MatchMachine(this);
        _cellMover = new CellMover(this);
    }

    private void Start()
    {
        InitializeBoard();
        VerifyBoardOnMatches();
        _cellFactory.InstantiateBoard(this, _cellMover);
    }

    private void Update()
    {
        List<Cell> finishedUpdating = new();
        _cellMover.Update();
        foreach (Cell cell in _updatingCells)
        {
            if (!cell.UpdateCell())
                finishedUpdating.Add(cell);
        }

        foreach (Cell cell in finishedUpdating)
        {
            int x = cell.Point.X;
            _fillingCellsCountByColumn[x] = Mathf.Clamp(_fillingCellsCountByColumn[x] - 1, 0, Config.BoardWidth);

            CellFlip flip = GetFlip(cell);
            List<Point> connectedPoints = _matchMachine.GetMatchedPoints(cell.Point, true);
            Cell flippedCell = null;

            if (flip != null)
            {
                flippedCell = flip.GetOtherCell(cell);
                MatchMachine.AddPoints(ref connectedPoints, _matchMachine.GetMatchedPoints(flippedCell.Point, true));
            }

            if (connectedPoints.Count == 0)
            {
                if (flippedCell != null)
                    FlipCells(cell.Point, flippedCell.Point, false);
            }
            else
            {
                ParticleSystem matchFx;
                if (_matchFxs.Count > 0 && _matchFxs[0].isStopped)
                {
                    matchFx = _matchFxs[0];
                    _matchFxs.RemoveAt(0);
                }
                else
                {
                    matchFx = Instantiate(_matchFxPrefab, transform);
                }
                _matchFxs.Add(matchFx);
                matchFx.Play();
                matchFx.transform.position = cell.Rect.transform.position;

                foreach (var connectedPoint in connectedPoints)
                {
                    _cellFactory.KillCell(connectedPoint);
                    var cellAtPoint = GetCellAtPoint(connectedPoint);
                    var connectedCell = cellAtPoint.GetCell();
                    if (connectedCell != null)
                    {
                        connectedCell.gameObject.SetActive(false);
                        _deadCells.Add(connectedCell);
                    }
                    cellAtPoint.SetCell(null);
                }

                _scoreService.AddScore(connectedPoints.Count);

                ApplyGravityToBoard();
            }

            _flippedCells.Remove(flip);
            _updatingCells.Remove(cell);
        }
    }

    private void ApplyGravityToBoard()
    {
        for (int x = 0; x < Config.BoardWidth; x++)
        {
            for (int y = Config.BoardHeight - 1; y >= 0; y--)
            {
                Point point = new(x, y);
                CellData cellData = GetCellAtPoint(point);
                CellData.CellType cellTypeAtPoint = GetCellTypeAtPoint(point);

                if (cellTypeAtPoint != 0)
                {
                    continue;
                }

                for (int newY = y - 1; newY >= -1; newY--)
                {
                    Point nextPoint = new(x, newY);
                    CellData.CellType nextCellType = GetCellTypeAtPoint(nextPoint);
                    if (nextCellType == 0)
                    {
                        continue;
                    }

                    if (nextCellType != CellData.CellType.Hole)
                    {
                        CellData cellAtPoint = GetCellAtPoint(nextPoint);
                        Cell cell = cellAtPoint.GetCell();
                        cellData.SetCell(cell);
                        _updatingCells.Add(cell);
                        cellAtPoint.SetCell(null);
                    }
                    else
                    {
                        CellData.CellType cellType = GetRandomCellType();
                        Point fallPoint = new(x, -1 - _fillingCellsCountByColumn[x]);
                        Cell cell;
                        if (_deadCells.Count > 0)
                        {
                            var revivedCell = _deadCells[0];
                            revivedCell.gameObject.SetActive(true);
                            cell = revivedCell;
                            _deadCells.RemoveAt(0);
                        }
                        else
                        {
                            cell = _cellFactory.InstantiateCell();
                        }

                        cell.Initialize(new CellData(cellType, point), _cellSprites[(int)(cellType - 1)], _cellMover);
                        cell.Rect.anchoredPosition = GetBoardPositionFromPoint(fallPoint);

                        CellData holeCell = GetCellAtPoint(point);
                        holeCell.SetCell(cell);
                        ResetCell(cell);
                        _fillingCellsCountByColumn[x]++;
                    }
                    break;
                }

            }
        }
    }

    public void FlipCells(Point firstPoint, Point secondPoint, bool main)
    {
        if (GetCellTypeAtPoint(firstPoint) < 0)
        {
            return;
        }

        var firstCellData = GetCellAtPoint(firstPoint);
        var firstCell = firstCellData.GetCell();
        if (GetCellTypeAtPoint(secondPoint) > 0)
        {
            var secondCellData = GetCellAtPoint(secondPoint);
            var secondCell = secondCellData.GetCell();
            firstCellData.SetCell(secondCell);
            secondCellData.SetCell(firstCell);

            if (main)
            {
                _flippedCells.Add(new CellFlip(firstCell, secondCell));
            }

            _updatingCells.Add(firstCell);
            _updatingCells.Add(secondCell);
        }
        else
        {
            ResetCell(firstCell);
        }
    }

    private CellFlip GetFlip(Cell cell)
    {
        foreach (CellFlip flip in _flippedCells)
        {
            if (flip.GetOtherCell(cell) != null)
            {
                return flip;
            }
        }
        return null;
    }

    public void ResetCell(Cell cell)
    {
        cell.ResetPosition();
        _updatingCells.Add(cell);
    }

    private void VerifyBoardOnMatches()
    {
        for (int y = 0; y < Config.BoardHeight; y++)
        {
            for (int x = 0; x < Config.BoardWidth; x++)
            {
                var point = new Point(x, y);
                var cellTypeAtPoint = GetCellTypeAtPoint(point);
                if (cellTypeAtPoint <= 0)
                {
                    continue;
                }

                var removeCellTypes = new List<CellData.CellType>();
                while (_matchMachine.GetMatchedPoints(point, true).Count > 0)
                {
                    if (removeCellTypes.Contains(cellTypeAtPoint) == false)
                    {
                        removeCellTypes.Add(cellTypeAtPoint);
                    }
                    SetCellTypeAtPoint(point, GetNewCellType(ref removeCellTypes));
                }
            }
        }
    }

    private void SetCellTypeAtPoint(Point point, CellData.CellType newCellType)
    {
        _boards[point.X, point.Y].NewCellType = newCellType;
    }

    private CellData.CellType GetNewCellType(ref List<CellData.CellType> removeCellTypes)
    {
        var availableCellTypes = new List<CellData.CellType>();
        for (int i = 0; i < CellSprites.Length; i++)
        {
            availableCellTypes.Add((CellData.CellType)i + 1);
        }
        foreach (var removeCellType in removeCellTypes)
        {
            availableCellTypes.Remove(removeCellType);
        }

        return availableCellTypes.Count <= 0 ? CellData.CellType.Blank :
            availableCellTypes[Random.Range(0, availableCellTypes.Count)];
    }

    public CellData.CellType GetCellTypeAtPoint(Point point)
    {
        if (point.X < 0 || point.X >= Config.BoardWidth || point.Y < 0 || point.Y >= Config.BoardHeight)
        {
            return CellData.CellType.Hole;
        }
        return _boards[point.X, point.Y].NewCellType;
    }

    private void InitializeBoard()
    {
        _boards = new CellData[Config.BoardWidth, Config.BoardHeight];
        for (int y = 0; y < Config.BoardHeight; y++)
        {
            for (int x = 0; x < Config.BoardWidth; x++)
            {
                _boards[x, y] = new CellData(BoartLayout.RowDatas[y].Rows[x] ? CellData.CellType.Hole : GetRandomCellType(), new Point(x, y));
            }
        }
    }

    private CellData.CellType GetRandomCellType() => (CellData.CellType)(Random.Range(0, _cellSprites.Length) + 1);

    public CellData GetCellAtPoint(Point point) => _boards[point.X, point.Y];

    public static Vector2 GetBoardPositionFromPoint(Point point)
    {
        return new Vector2(Config.PieceSize / 2 + Config.PieceSize * point.X, -Config.PieceSize / 2 - Config.PieceSize * point.Y);
    }
}