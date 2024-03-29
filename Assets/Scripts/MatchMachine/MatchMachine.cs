using System;
using System.Collections.Generic;

public class MatchMachine
{
    private readonly BoardService _boardService;

    private readonly Point[] _directions =
    {
        Point.Up, Point.Right, Point.Down, Point.Left
    };

    public MatchMachine(BoardService boardService)
    {
        _boardService = boardService;
    }

    public List<Point> GetMatchedPoints(Point point, bool main)
    {
        var connectedPoints = new List<Point>();
        var cellTypeAtPoint = _boardService.GetCellTypeAtPoint(point);

        CheckForDirectionMatch(ref connectedPoints, point, cellTypeAtPoint);
        CheckForMiddDirectionMatch(ref connectedPoints, point, cellTypeAtPoint);
        CheckForSquareDirectionMatch(ref connectedPoints, point, cellTypeAtPoint);

        if (main)
        {
            for (int i = 0; i < connectedPoints.Count; i++)
            {
                AddPoints(ref connectedPoints, GetMatchedPoints(connectedPoints[i], false));
            }
        }

        return connectedPoints;
    }

    private void CheckForSquareDirectionMatch(ref List<Point> connectedPoints, Point point, CellData.CellType cellTypeAtPoint)
    {
        for (int i = 0; i < 4; i++)
        {
            List<Point> square = new List<Point>();

            int nextCellIndex = i + 1;
            nextCellIndex = nextCellIndex > 3 ? 0 : nextCellIndex;

            Point[] checkPoints =
            {
                Point.Add(point, _directions[i]),
                Point.Add(point, _directions[nextCellIndex]),
                Point.Add(point, Point.Add(_directions[i], _directions[nextCellIndex])),
            };

            foreach (Point checkPoint in checkPoints)
            {
                if(_boardService.GetCellTypeAtPoint(checkPoint) == cellTypeAtPoint)
                {
                    square.Add(checkPoint); 
                }
            }

            if(square.Count > 2)
            {
                AddPoints(ref connectedPoints, square);
            }
        }
    }

    private void CheckForMiddDirectionMatch(ref List<Point> connectedPoints, Point point, CellData.CellType cellTypeAtPoint)
    {
        for (int i = 0; i < 2; i++)
        {
            List<Point> line = new List<Point>();

            Point[] checkPoints =
            {
                Point.Add(point, _directions[i]),
                Point.Add(point, _directions[i + 2]),
            };

            foreach (Point checkPoint in checkPoints)
            {
                if(_boardService.GetCellTypeAtPoint(checkPoint) == cellTypeAtPoint)
                {
                    line.Add(checkPoint);
                }
            }

            if(line.Count > 1)
            {
                AddPoints(ref connectedPoints, line);
            }
        }
    }

    private void CheckForDirectionMatch(ref List<Point> connectedPoints, Point point, CellData.CellType cellTypeAtPoint)
    {
        foreach (Point direction in _directions)
        {
            List<Point> line = new List<Point>();

            for (int i = 1; i < 3; i++)
            {
                Point checkPoint = Point.Add(point, Point.Multiply(direction, i));
                if (_boardService.GetCellTypeAtPoint(checkPoint) == cellTypeAtPoint)
                {
                    line.Add(checkPoint);
                }
            }

            if (line.Count > 1)
            {
                AddPoints(ref connectedPoints, line);
            }
        }
    }

    public static void AddPoints(ref List<Point> points, List<Point> addPoints)
    {
        foreach (Point addPoint in addPoints)
        {
            bool doAdd = true;
            foreach (Point point in points)
            {
                if (point.Equals(addPoint))
                {
                    doAdd = false;
                    break;
                }
            }
            if (doAdd)
            {
                points.Add(addPoint);
            }
        }
    }
}