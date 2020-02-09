using System;

namespace Tetris
{
    class GameField
    {
        public readonly Point Sizes = new Point(10, 20);
        public readonly string[] Lines = new string[20];

        public GameField() 
        {
            FillEmpty();
        }

        /// <summary>
        /// Заполнить игровое поле пустыми символами
        /// (очистить)
        /// </summary>
        public void FillEmpty() 
        {
            for (int i = 0; i < Sizes.Y; i++)
            {
                Lines[i] = new string(' ', Sizes.X);
            }
        }


        /// <summary>
        /// Проверить, что фигура находится в границах игрового поля
        /// </summary>
        /// <returns>TRUE или FALSE в зависимости от того,
        /// находится ли фигура в границах игрового поля</returns>
        public bool CheckLimits(IFigure figure, Point point = null)
        {
            Point pos;
            if (point == null)
            {   // offset X: 1 > 0
                pos = new Point(figure.Position.X, figure.Position.Y);
            }
            else
            {   // offset X: 1 > 0
                pos = new Point(point.X, point.Y);
            }

            return (pos.X > 0 && pos.Y > 0) &&
                pos.X + figure.Data[0].Length - 1 <= Sizes.X && pos.Y + figure.Data.Length - 1 <= Sizes.Y;
                

        }

        /// <summary>
        /// Проверить коллизии для фигуры
        /// </summary>
        /// <param name="figure">Конкретная фигура</param>
        /// <param name="point">Ее позиции (опционально)</param>
        /// <returns>TRUE или FALSE в зависимости от того, касается ли 
        /// фигура нижней части поля или других фигур</returns>
        public bool CheckCollisions(IFigure figure, Point point = null)
        {
            Point pos;
            if (point == null)
            {   // offset X: 1 > 0
                pos = new Point(figure.Position.X - 1, figure.Position.Y);
            }
            else
            {   // offset X: 1 > 0
                pos = new Point(point.X - 1, point.Y);
            }

            for (int y = 0; y < figure.Data.Length; y++)
            {
                for (int x = 0; x < figure.Data[0].Length; x++)
                {
                    if (figure.Data[y][x] == ' ')
                    {
                        continue;
                    }

                    if (Lines[pos.Y + y][pos.X + x] != ' ')
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Разместить фигуру на игровом поле
        /// Нужно например, когда фигура с чем-то столкнулась
        /// Тогда фигура станет частью игрового поля
        /// </summary>
        /// <param name="figure">Какую фигуру разместить</param>
        /// <returns>TRUE или FALSE в зависимости от того, 
        /// Получилось ли разместить фигуру</returns>
        public bool PlaceFigure(IFigure figure)
        {
            var pos = new Point(figure.Position.X - 1, figure.Position.Y);

            // проверить все пределы
            if (pos.X < 0 || pos.Y < 0 || 
                pos.X + figure.Data[0].Length > Sizes.X || 
                pos.Y + figure.Data.Length > Sizes.Y)
            {
                return false;
            }

            var data = Rotate(figure);

            for (int y = 0; y < data.Length; y++)
            {
                var tString = Lines[pos.Y + y].ToCharArray();

                // операция очень проста - на игровом 
                // поле меняем только нужные нам символы
                for (int x = 0; x < data[0].Length; x++)
                {
                    tString[pos.X + x] = data[y][x];
                }

                // записываем новую строку на игровую карту
                Lines[y + pos.Y] = new string(tString);
            }

            return true;
        }

        /// <summary>
        /// Вывести все то, что сейчас на экране
        /// </summary>
        /// <param name="clear">Очищать ли экран дополнительно, перед операцией</param>
        public void DrawScreen(bool clear = true) 
        {
            if(clear) Console.Clear();

            for (int i = 0; i < Sizes.Y; i++)
            {
                Console.WriteLine(Lines[i]);
            }
        }

        public int CheckLine(char checkFor = '#') 
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i] == new string(checkFor, Sizes.X))
                {
                    return i;
                }    
            }
            return -1;
        }

        public void DeleteLine(int id) 
        { 
            if(id < 0 || id > Lines.Length)
            {
                return;
            }

            var t = new string[id];

            for (int i = 0; i < id; i++)
            {
                t[i] = Lines[i];
            }

            for (int i = 1; i <= id; i++)
            {
                Lines[i] = t[i-1];
            }

            Lines[0] = new string(' ', 10);
            
        }

        public string[] Rotate(IFigure figure)
        {
            string[] tdata = figure.Data;

            int rotate = figure.RotateLength;

            if (rotate > 0)
            {
                figure.RotateLength = 0;

                for (int r = 0; r < rotate; r++)
                {
                    tdata = new string[figure.Data[0].Length];

                    for (int i = 0; i < figure.Data[0].Length; i++)
                    {
                        string tstr = string.Empty;

                        for (int j = figure.Data.Length-1; j >= 0; j--)
                        {
                            tstr += figure.Data[j][i];
                        }
                        tdata[i] = tstr;
                    }

                    figure.Data = new string[tdata.Length];
                    Array.Copy(tdata, figure.Data, tdata.Length);

                }
            }

            return tdata;
        }

        public void DrawFigure(IFigure figure)
        {
            var pos = new Point(figure.Position.X - 1, figure.Position.Y);

            var data = Rotate(figure);

            var cursorPos = new Point(Console.CursorTop, Console.CursorLeft);
            for (int i = 0; i < data.Length; i++)
            {
                Console.SetCursorPosition(pos.X, pos.Y + i);
                for (int j = 0; j < data[0].Length; j++)
                {
                    Console.Write(data[i][j]);
                }
            }
                
            Console.SetCursorPosition(cursorPos.X, cursorPos.Y);
        }
    }
}
