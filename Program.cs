using System;
using System.Threading;

namespace Tetris
{
    public class Program
    {
        private static GameField _field = new GameField(Console.BufferWidth);
        private static IFigure _figure = TakeRandom();
        private static int Lines = 0;
        
        static void Main()
        {
            int currentTick = 0;
            int maxTick = 20;
            int controlTick = maxTick / 4;
            
            do
            {
                Thread.Sleep(10);
                currentTick++;

                if (currentTick % maxTick == 0)
                {

                    int fillId = _field.FindFullLine();
                    if (fillId != -1)
                    {
                        _field.DeleteLine(fillId);
                        Lines++;
                    }

                    if (_field.CheckContainsLine(0, '#'))
                    {
                        _field.FillField(' ');
                        Lines = 0;
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
                        _figure = TakeRandom();
                    }

                    currentTick = 0; // сброс таймера
                }

                if (currentTick % controlTick == 0)
                {
                    // отрисовка всего экрана
                    _field.DrawScreen();
      
                    // отрисовка статистики и т.д
                    DrawStats();
                    
                    _field.HandleRotate(_figure);
                    
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
            Console.WriteLine(" HovyTetris  \t {0} ", "0.15");
            Console.SetCursorPosition(7, 6);
            Console.WriteLine(" Cur. Speed  \t {0}   ", "25");
            Console.SetCursorPosition(7, 7);
            Console.WriteLine(" Burn Lines  \t {0}   ", Lines);
            Console.SetCursorPosition(7, 15);
            Console.WriteLine(".Game Design by");
            Console.SetCursorPosition(7, 16);
            Console.WriteLine("Alexey Pajitnov");
        }
    }
}