using StaticData;
using System;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CellFactory))]
public class BoardService : MonoBehaviour
{
    [SerializeField] private Sprite[] _cellSprites;

    private CellData[,] _boards;
    private CellFactory _cellFactory;

    public ArrayLayout BoartLayout;

    public Sprite[] CellSprites => _cellSprites;

    private void Awake()
    {
        _cellFactory = GetComponent<CellFactory>();
    }

    private void Start()
    {
        InitializeBoard();
        _cellFactory.InstantiateBoard(this);
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

    public Vector2 GetBoardPositionFromPoint(Point point)
    {
        return new Vector2(Config.PieceSize / 2 + Config.PieceSize * point.X, -Config.PieceSize / 2 - Config.PieceSize * point.Y);
    }
}
