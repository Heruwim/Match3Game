using UnityEngine;

public class CellMover
{
    public void MoveCell(Cell cell)
    {
        Debug.Log($"{cell.Point.X}, {cell.Point.Y}");
    }

    public void DropCell()
    {
        Debug.Log("drop");
    }
}
