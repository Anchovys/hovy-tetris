using System;
using System.Threading;

namespace Tetris
{
    public class Program
    {
        private static GameField _field = new GameField(Console.BufferWidth);
        
        private static IFigure _figure = TakeRandom();
        private static IFigure _nextFigure = TakeRandom();
        
        private static int Lines;
        private static int Speed; 
        
        static void Main()
        {
            Console.CursorVisible = false;
            
            // текущий тик
            int currentTick = 0;
            // частота отрисовки карты
            int renderTick = 10;
            // частота обработки управления
            int controlTick = renderTick / 5;

            DrawNext();
            
            do
            {
                Thread.Sleep(20 - Speed);
                currentTick++;

                if (currentTick % renderTick == 0)
                {
                    // попытка сжечь линию
                    int fillId = _field.FindFullLine();
                    if (fillId != -1) // есть такая
                    {
                        _field.DeleteLine(fillId); // сжигаем
                        Lines++;                   // записываем в очки
                        
                        
                        // тут же изменяем скорость игры
                        if (Lines % 2 == 0 && Speed < 15)
                        {
                            Speed--;
                        }

                    }

                    // проверка на провал
                    if (_field.CheckContainsLine(0, '#'))
                    {
                        _field.FillField(' '); // очищаем поле
                        Lines = 0;                    // сброс очков
                    }

                    var _pos = new Point(_figure.Position.X, _figure.Position.Y);
                    _pos.Y++; // смещение фигуры вниз
                        
                    if (_field.CheckLimits(_figure, _pos) && _field.CheckCollision(_figure, _pos))
                    {
                        _figure.Position = _pos; // смещаем фигуру вниз
                    }
                    else
                    {
                        _field.PlaceFigure(_figure);
                        _figure = _nextFigure;
                        _nextFigure = TakeRandom();
                        
                        DrawNext();
                        
                    }

                    // отрисовка статистики и т.д
                    DrawStats();
                    
                    currentTick = 0; // сброс таймера
                }

                if (currentTick % controlTick == 0)
                {
                    // отрисовка всего экрана
                    _field.DrawScreen();

                    // нужно повернуть фигуру
                    if (_figure.RotateLength > 0)
                    {
                        // обрабатываем поворот
                        var tempR = _field.HandleRotate(_figure);
                        
                        // ничему не будет мешать
                        if (_field.CheckLimits(tempR, _figure.Position) && 
                            _field.CheckCollision(tempR, _figure.Position)) 
                        {
                            _figure.Data = tempR; // применяем поворот
                        }
                    }

                    // отрисовка падающей фигуры (отдельно)
                    _field.DrawFigure(_figure);

                    // никакую кнопку не обрабатываем
                    if (!Console.KeyAvailable)
                    {
                        continue; // выходим
                    }

                    // в зависимости от того, какую кнопку зажали
                    ConsoleKey key = Console.ReadKey().Key;

                    if (key == ConsoleKey.A)
                    {
                        var _pos = new Point(_figure.Position.X, _figure.Position.Y);
                        _pos.X--; // смещение фигуры влево
                        
                        if (_field.CheckLimits(_figure, _pos) && _field.CheckCollision(_figure, _pos))
                        {
                            _figure.Position = _pos; // применяем новую позицию
                        }
                    }
                    else if (key == ConsoleKey.D)
                    {
                        var _pos = new Point(_figure.Position.X, _figure.Position.Y);
                        _pos.X++; // смещение фигуры вправо

                        if (_field.CheckLimits(_figure, _pos) && _field.CheckCollision(_figure, _pos))
                        {
                            _figure.Position = _pos; // применяем новую позицию
                        }
                    }
                    else if (key == ConsoleKey.R)
                    {
                        _figure.RotateLength++;
                    }
                }
            } while (true);
        }
        
        static IFigure TakeRandom()
        {
            var random = new Random();
            var figures = new []
            {
                new Teewee           { Position =  new Point(4, 0) } as IFigure, 
                new Hero             { Position =  new Point(4, 0) } as IFigure,
                new Square           { Position =  new Point(4, 0) } as IFigure,
                new OrangeRicky      { Position =  new Point(4, 0) } as IFigure, 
                new BlueRicky        { Position =  new Point(4, 0) } as IFigure,
                new PhodeIslandLeft  { Position =  new Point(4, 0) } as IFigure, 
                new PhodeIslandRight { Position =  new Point(4, 0) } as IFigure, 
            };

            return figures[random.Next(figures.Length)].Clone() as IFigure;
        }

        static void DrawStats()
        {
            Console.SetCursorPosition(7, 5);
            Console.WriteLine(" HovyTetris  \t {0} ", "0.16");
            Console.SetCursorPosition(7, 6);
            Console.WriteLine(" Cur. Speed  \t {0}   ", Speed + 1);
            Console.SetCursorPosition(7, 7);
            Console.WriteLine(" Burn Lines  \t {0}   ", Lines);
            Console.SetCursorPosition(7, 15);
            Console.WriteLine(".Game Design by");
            Console.SetCursorPosition(7, 16);
            Console.WriteLine("Alexey Pajitnov");
        }

        static void DrawNext()
        {

            
            int offsetX = 6;
            int offsetY = 3;
            
            for (int y = 0; y < 6; y++)
            {
                Console.SetCursorPosition(_field.GlobalOffsetX + _field.Sizes.X + 2, 5 + y);
                for (int x = 0; x < 12; x++)
                {
                    int xpos = x - offsetX;
                    int ypos = y - offsetY;
                    
                    if (xpos < _nextFigure.Data[0].Length && xpos >= 0 && 
                        ypos < _nextFigure.Data.Length && ypos >= 0)
                    {
                        Console.Write(_nextFigure.Data[ypos][xpos]);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
            }
        }
    }
}