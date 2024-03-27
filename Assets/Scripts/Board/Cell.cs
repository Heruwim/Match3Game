using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] private Image _image;

    private CellType _cellType;
    private Point _point;

    public RectTransform Rect;

    public enum CellType
    {
        Hole = -1,
        Blank = 0,
        Apple = 1,
        Banana = 2,
        BlueBerry = 3,
        Grape = 4,
        Orange = 5,
        Pear = 6,
        Strawberry = 7,
    }

    public void Initialize(CellType cellType, Point point, Sprite sprite)
    {
        _cellType = cellType;
        _point = point;
        _image.sprite = sprite;
    }
}
