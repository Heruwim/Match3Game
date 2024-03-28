using StaticData;
using System;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardService : MonoBehaviour
{
    [SerializeField] private RectTransform _BoardRect;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private Sprite[] _cellSprites;

    private CellData[,] _boards;

    public ArrayLayout BoartLayout;

    private void Start()
    {
        InitializeBoard();
        InstantiateBoard();
    }

    private void InstantiateBoard()
    {
        for (int y = 0; y < Config.BoardHeight; y++)
        {
            for (int x = 0; x < Config.BoardWidth; x++)
            {
                Point point = new Point(x, y);
                CellData cellData = GetCellAtPoint(point);
                var cellType = cellData.NewCellType;
                if(cellType <= 0)
                {
                    continue;
                }

                Cell cell = InstantiateCell();
                cell.Rect.anchoredPosition = GetBoardPositionFromPoint(point);
                cell.Initialize(cellData, _cellSprites[(int)(cellType - 1)]);

            }
        }
    }

    private CellData GetCellAtPoint(Point point) => _boards[point.X, point.Y];

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

    private Cell InstantiateCell() => Instantiate(_cellPrefab, _BoardRect);

    private Vector2 GetBoardPositionFromPoint(Point point)
    {
        return new Vector2(Config.PieceSize / 2 + Config.PieceSize * point.X, -Config.PieceSize / 2 - Config.PieceSize * point.Y);
    }
}
