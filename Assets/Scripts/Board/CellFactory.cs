using StaticData;
using UnityEngine;

public class CellFactory : MonoBehaviour
{
    [SerializeField] private RectTransform _BoardRect;
    [SerializeField] private Cell _cellPrefab;

    private BoardService _boardService;

    public void InstantiateBoard(BoardService boardService, CellMover cellMover)
    {
        for (int y = 0; y < Config.BoardHeight; y++)
        {
            for (int x = 0; x < Config.BoardWidth; x++)
            {
                Point point = new Point(x, y);
                CellData cellData = boardService.GetCellAtPoint(point);
                var cellType = cellData.NewCellType;
                if (cellType <= 0)
                {
                    continue;
                }

                Cell cell = InstantiateCell();
                cell.Rect.anchoredPosition = boardService.GetBoardPositionFromPoint(point);
                cell.Initialize(cellData, boardService.CellSprites[(int)(cellType - 1)], cellMover);

            }
        }
    }    

    private Cell InstantiateCell() => Instantiate(_cellPrefab, _BoardRect);
}