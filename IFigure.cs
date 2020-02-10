namespace TetrisBool
{
    public interface IFigure
    {
        public bool[,] Data { get; set; }
        public Point Position { get; set; }
        public int Rotate { get; set; }
    }
}