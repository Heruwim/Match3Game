using StaticData;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CellFactory))]
public class BoardService : MonoBehaviour
{
    [SerializeField] private Sprite[] _cellSprites;

    private CellData[,] _boards;
    private CellFactory _cellFactory;
    private MatchMachine _matchMachine;
    private CellMover _cellMover;
    private readonly List<Cell> _updatingCells = new List<Cell>();
    private readonly List<CellFlip> _flippedCells = new List<CellFlip>();

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
        List<Cell> finishedUpdating = new List<Cell>();
        _cellMover.Update();
        foreach (Cell cell in _updatingCells)
        {
            if (!cell.UpdateCell())
                finishedUpdating.Add(cell);
        }

        foreach (Cell cell in finishedUpdating)
        {
            var flip = GetFlip(cell);
            _flippedCells.Remove(flip);
            _updatingCells.Remove(cell);
        }
    }

    public void FlipCells(Point firstPoint, Point secondPoint, bool main)
    {
        if(GetCellTypeAtPoint(firstPoint) < 0)
        {
            return;
        }

        var firstCellData = GetCellAtPoint(firstPoint);
        var firstCell = firstCellData.GetCell();
        if(GetCellTypeAtPoint(secondPoint) > 0)
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