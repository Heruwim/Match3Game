using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] private Image _image;
    
    private CellData _cellData;

    public RectTransform Rect;

    public Point Point => _cellData.NewPoint;
    public CellData.CellType CellType => _cellData.NewCellType; 

    public void Initialize(CellData cellData, Sprite sprite)
    {
        _cellData = cellData;
        _image.sprite = sprite;
        UpdateName();
    }

    private void UpdateName() => transform.name = $"Cell [{Point.X}, {Point.Y}]";
}