using System;

public class CellData
{
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

    private Cell _cell;

    public CellType NewCellType;
    public Point NewPoint;

    public CellData(CellType cellType, Point point)
    {
        NewCellType = cellType;
        NewPoint = point;
    }

    public Cell GetCell() => _cell;

    public void SetCell(Cell newCell)
    {
        _cell = newCell;
        if (_cell == null)
        {
            NewCellType = CellType.Blank;
        }
        else
        {
            NewCellType = newCell.CellType;
            _cell.SetCellPoint(NewPoint);
        }
    }
}