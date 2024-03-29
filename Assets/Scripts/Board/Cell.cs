using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private float _moveSpeed = 10f;
    
    private CellData _cellData;
    private CellMover _cellMover;
    private Vector2 _position;
    private bool _isUpdating;

    public RectTransform Rect;

    public Point Point => _cellData.NewPoint;
    public CellData.CellType CellType => _cellData.NewCellType; 

    public void Initialize(CellData cellData, Sprite sprite, CellMover cellMover)
    {
        _cellData = cellData;
        _image.sprite = sprite;
        _cellMover = cellMover;
    }

    public bool UpdateCell()
    {
        if(Vector3.Distance(Rect.anchoredPosition, _position) < 1 )
        {
            MoveToPosition(_position);
            _isUpdating = true;
        }
        else
        {
            Rect.anchoredPosition = _position;
            _isUpdating = false;
        }

        return _isUpdating;
    }

    private void UpdateName() => transform.name = $"Cell [{Point.X}, {Point.Y}]";

    public void OnPointerDown(PointerEventData eventData)
    => _cellMover.MoveCell(this);

    public void OnPointerUp(PointerEventData eventData) => _cellMover.DropCell();

    public void MoveToPosition(Vector2 newPointPosotion) 
        => Rect.anchoredPosition = Vector2.Lerp(Rect.anchoredPosition, newPointPosotion, _moveSpeed * Time.deltaTime);

    public void ResetPosition() => _position = BoardService.GetBoardPositionFromPoint(Point);

    public void SetCellPoint(Point point)
    {
        _cellData.NewPoint = point;
        UpdateName();
        ResetPosition();
    }
}