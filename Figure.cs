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
        }

        void FallDown();
        void MoveLeft();
        void MoveRight();
        
    }
}
