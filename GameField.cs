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
                pos = new Point(figure.Position.X - 2, figure.Position.Y);
            }
            else
            {   // offset X: 1 > 0
                pos = new Point(point.X - 2, point.Y);
            }

            if (pos.Y + figure.Data.Length > Sizes.Y)
            {
                return false;
            }

            for (int y = 0; y < figure.Data.Length; y++)
            {
                for (int x = 0; x < figure.Data[y].Length; x++)
                {
                    if (figure.Data[y][x] == ' ')
                    {
                        continue;
                    }

                    if (Lines[pos.Y + y][pos.X + x + 1] != ' ')
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

            for (int y = 0; y < figure.Data.Length; y++)
            {
                var tString = Lines[pos.Y + y].ToCharArray();

                // операция очень проста - на игровом 
                // поле меняем только нужные нам символы
                for (int x = 0; x < figure.Data[0].Length; x++)
                {
                    tString[pos.X + x] = figure.Data[y][x];
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

        public int CheckLine() 
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i] == new string('#', Sizes.X))
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

            for (int i = 1; i < Lines.Length; i++)
            {
                Lines[i] = Lines[i - 1];
            }
            Lines[0] = new string(' ', Sizes.X);
        }

        public void DrawFigure(IFigure figure)
        {
            var pos = new Point(figure.Position.X - 1, figure.Position.Y);

            var cursorPos = new Point(Console.CursorTop, Console.CursorLeft);
            for (int i = 0; i < figure.Data.Length; i++)
            {
                Console.SetCursorPosition(pos.X, pos.Y + i);
                Console.Write(figure.Data[i]);
            }
                
            Console.SetCursorPosition(cursorPos.X, cursorPos.Y);
        }
    }
}
