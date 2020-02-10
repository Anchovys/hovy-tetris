namespace TetrisBool
{
    /// <summary>
    /// Нужно для хранения удобного X и Y
    /// </summary>
    public class Point
    {
        public int X;
        public int Y;

        public static Point Empty = new Point(0, 0);
        
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
