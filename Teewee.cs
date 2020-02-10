namespace TetrisBool
{
    public class Teewee : IFigure
    {
        private Point _point;
        private bool[,] _data = new bool[,]
        {
            {true, true},
            {true, true},
        };

        public Teewee(Point point)
        {
            _point = point;
        }
        
        public bool[,] Data
        {
            get => _data;
            set => _data = value;
        }
        
        public Point Position 
        {
            get => _point;
            set => _point = value;
        }
    }
}