using System;

namespace Tetris
{
    class Teewee : IFigure
    {
        string[] data = {
            " # ",
            "###"
        };

        int rotateLength = 0;

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
            set
            {
                data = value;
            }
        }

        public int RotateLength
        {
            get
            {
                return rotateLength;
            }
            set
            {
                rotateLength = value;
            }
        }

        #endregion
        
        public object Clone()
        {
            return (IFigure) MemberwiseClone();
        }
    }
}
