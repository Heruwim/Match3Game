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

        if (main)
        {
            for (int i = 0; i < connectedPoints.Count; i++)
            {
                AddPoints(ref connectedPoints, GetMatchedPoints(connectedPoints[i], false));
            }
        }

        return connectedPoints;
    }

    private void CheckForDirectionMatch(ref List<Point> connectedPoints, Point point, CellData.CellType cellTypeAtPoint)
    {
        foreach (var direction in _directions)
        {
            var line = new List<Point>();

            for (int i = 1; i < 3; i++)
            {
                var checkPoint = Point.Add(point, Point.Multiply(direction, i));
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

    private static void AddPoints(ref List<Point> points, List<Point> addPoints)
    {
        foreach (var addPoint in addPoints)
        {
            bool doAdd = true;
            foreach (var point in points)
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