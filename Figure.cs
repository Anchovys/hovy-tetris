using System;

namespace Tetris
{
    interface IFigure : ICloneable
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

        object Clone();

    }
}
