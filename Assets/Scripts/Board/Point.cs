using UnityEngine;

[System.Serializable]
public class Point
{
    public int X;
    public int Y;

    public Point(int newX, int newY) 
    {  
        X = newX; 
        Y = newY;
    }

    public void Multiply(int value)
    {
        X *= value;
        Y *= value;
    }

    public void Add(Point point)
    {
        X += point.X;
        Y += point.Y;
    }

    public bool Equals(Point point) => X == point.X && Y == point.Y;
    
    public Vector2 ToVector() => new Vector2(X, Y);

    public static Point FromVector(Vector2 vector) => new ((int)vector.x, (int)vector.y);

    public static Point FromVector(Vector3 vector) => new ((int)vector.x, (int)vector.y);

    public static Point Multiply(Point point, int value) => new (point.X * value, point.Y * value);

    public static Point Add(Point point1, Point point2) => new (point1.X + point2.X, point1.Y + point2.Y);

    public static Point Clone(Point point) => new (point.X, point.Y);

    public static Point Zero => new Point(0, 0);

    public static Point Up => new Point(0, 1);

    public static Point Down => new Point(0, -1);

    public static Point Left => new Point(-1, 0);

    public static Point Right => new Point(1, 0);
}
