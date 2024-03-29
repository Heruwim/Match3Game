using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _image;
    
    private CellData _cellData;
    private CellMover _cellMover;

    public RectTransform Rect;

    public Point Point => _cellData.NewPoint;
    public CellData.CellType CellType => _cellData.NewCellType; 

    public void Initialize(CellData cellData, Sprite sprite, CellMover cellMover)
    {
        _cellData = cellData;
        _image.sprite = sprite;
        _cellMover = cellMover;
        UpdateName();
    }

    private void UpdateName() => transform.name = $"Cell [{Point.X}, {Point.Y}]";

    public void OnPointerDown(PointerEventData eventData)
    {
        _cellMover.MoveCell(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _cellMover.DropCell();
    }
}