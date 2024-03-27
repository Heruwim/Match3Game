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

    private void Start()
    {
        for (int x = 0; x < Config.BoardWidth; x++)
        {
            for(int y = 0; y < Config.BoardHeight; y++)
            {
                Cell cell = InstantiateCell();
                Point point = new Point(x, y);
                cell.Rect.anchoredPosition = GetBoardPositionFromPoint(point);
                var cellType = GetRandomCellType();
                cell.Initialize(new CellData(cellType, point), _cellSprites[(int)(cellType - 1)]);

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
