using System;

namespace Tetris
{
    class GameField
    {
        private readonly Point _sizes = new Point(10, 20);
        private readonly char[][] _lines = new char[20][];
        private readonly int _globalOffsetX = 20;

        public GameField(int bufferWidth, Point customSizes = null)
        {
            if (customSizes != null)
            {
                _sizes = customSizes;
                _lines = new char[_sizes.Y][];
            }

            FillField(' ');
            
            for (int i = 0; i < _sizes.Y + 2; i++)
            {
                Console.WriteLine(new string('.', bufferWidth));
            }

            _globalOffsetX = bufferWidth / 2 - (_sizes.X / 2);

        }

        /// <summary>
        /// Заполнить игровое поле пустыми символами
        /// (очистить)
        /// </summary>
        public void FillField(char symbol = ' ') 
        {
            for (int i = 0; i < _sizes.Y; i++)
            {
                _lines[i] = new string(symbol, _sizes.X).ToCharArray();
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

            return (pos.X >= 0 && pos.Y >= 0) &&
                pos.X + figure.Data[0].Length <= _sizes.X && pos.Y + figure.Data.Length <= _sizes.Y;
                

        }

        /// <summary>
        /// Проверить коллизии для фигуры
        /// </summary>
        /// <param name="figure">Конкретная фигура</param>
        /// <param name="point">Ее позиции (опционально)</param>
        /// <returns>TRUE или FALSE в зависимости от того, касается ли 
        /// фигура нижней части поля или других фигур</returns>
        public bool CheckCollision(IFigure figure, Point point = null)
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

            for (int y = 0; y < figure.Data.Length; y++)
            {
                for (int x = 0; x < figure.Data[0].Length; x++)
                {
                    if (figure.Data[y][x] == ' ')
                    {
                        continue;
                    }

                    if (_lines[pos.Y + y][pos.X + x] != ' ')
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
            var pos = new Point(figure.Position.X, figure.Position.Y);

            // проверить все пределы
            if (pos.X < 0 || pos.Y < 0 || 
                pos.X + figure.Data[0].Length > _sizes.X || 
                pos.Y + figure.Data.Length > _sizes.Y)
            {
                return false;
            }

            for (int y = 0; y < figure.Data.Length; y++)
            {
                var tString = _lines[pos.Y + y];

                // операция очень проста - на игровом 
                // поле меняем только нужные нам символы
                for (int x = 0; x < figure.Data[0].Length; x++)
                {
                    if (tString[pos.X + x] != '#' && figure.Data[y][x] != ' ')
                    {
                        tString[pos.X + x] = figure.Data[y][x];
                    }
                }

                // записываем новую строку на игровую карту
                _lines[y + pos.Y] = new string(tString).ToCharArray();
            }

            return true;
        }

        /// <summary>
        /// Вывести все то, что сейчас на экране
        /// </summary>
        /// <param name="clear">Очищать ли экран дополнительно, перед операцией</param>
        public void DrawScreen(bool clear = true) 
        {
            if (clear)
            {
                for (int y = 0; y < _sizes.Y; y++)
                {
                    Console.SetCursorPosition(_globalOffsetX, y);
                    Console.WriteLine(new string('+', _sizes.X));
                }
            }
            
            for (int y = 0; y < _sizes.Y; y++)
            {
                Console.SetCursorPosition(_globalOffsetX, y);
                Console.WriteLine(_lines[y]);
            }
        }

        public int FindFullLine(char checkFor = '#') 
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                bool fill = true;
                for (int j = 0; j < _lines[i].Length; j++)
                {
                    if (_lines[i][j] != checkFor)
                        fill = false;
                }

                if (fill)
                    return i;

            }
            return -1;
        }

        public void DeleteLine(int id) 
        { 
            if(id < 0 || id > _lines.Length)
            {
                return;
            }

            var t = new char[id][];

            for (int i = 0; i < id; i++)
            {
                t[i] = _lines[i];
            }

            for (int i = 1; i <= id; i++)
            {
                _lines[i] = t[i-1];
            }

            _lines[0] = new string(' ', _sizes.X).ToCharArray();
            
        }

        public void HandleRotate(IFigure figure)
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

            figure.Data = tdata;
        }

        public void DrawFigure(IFigure figure)
        {
            var pos = new Point(figure.Position.X + _globalOffsetX, figure.Position.Y);

            var cursorPos = new Point(Console.CursorTop, Console.CursorLeft);
            for (int i = 0; i < figure.Data.Length; i++)
            {
                Console.SetCursorPosition(pos.X, pos.Y + i);
                for (int j = 0; j < figure.Data[0].Length; j++)
                {
                    Console.Write(figure.Data[i][j]);
                }
            }
                
            Console.SetCursorPosition(cursorPos.X, cursorPos.Y);
        }
    }
}
