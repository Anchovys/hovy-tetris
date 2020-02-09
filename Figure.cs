namespace Tetris
{
    interface IFigure
    {
        Point Position
        {
            get;
            set;
        }

        string[] Data // read only
        {
            get;
            set;
        }

        int RotateLength
        {
            get;
            set;
        }

        void FallDown();
        void MoveLeft();
        void MoveRight();
        void Rotate();

    }
}
