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

        public void FallDown()
        {
            Position.Y++;
        }
        public void MoveLeft()
        {
            Position.X--;
        }

        public void Rotate()
        {
            rotateLength++;
        }

        public void MoveRight()
        {
            Position.X++;
        }
    }
}
