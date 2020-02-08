namespace Tetris
{
    class Teewee : IFigure
    {
        readonly string[] data = {
            " # ",
            "###"
        };

        private Point position = new Point(0, 0);

        #region fields

        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public string[] Data
        {
            get
            {
                return data;
            }
        }

        #endregion

        public void FallDown()
        {
            position.Y++;
        }
        public void MoveLeft()
        {
            var pos = new Point(Position.X - 1, Position.Y - 1);

            if (pos.X < 0)
                return;

            position.X--;
        }

        public void MoveRight()
        {
            if (Position.X + Data[0].Length > 10)
                return;

            position.X++;
        }
    }
}
