using System;
using System.Threading;

namespace Tetris
{
    public class Program
    {
        private static readonly GameField _field = new GameField(Console.BufferWidth);
        
        private static IFigure _figure = TakeRandom();
        private static IFigure _nextFigure = TakeRandom();
        
        private static int Lines;
        private static int Speed; 
        
        static void Main()
        {
            Console.CursorVisible = false;

            Console.WriteLine("HovyTetris" +
                              "\nTap [enter] for start!");
            Console.ReadLine();
            
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
                        Lines++; // записываем в очки


                        // тут же изменяем скорость игры
                        if (Lines % 2 == 0 && Speed < 15)
                        {
                            Speed++;
                        }

                    }

                    // проверка на провал
                    if (_field.CheckContainsLine(0, '#'))
                    {
                        _field.FillField(' '); // очищаем поле
                        Lines = 0; // сброс очков
                        Speed = 0; // сброс скорости
                        
                        Console.SetCursorPosition(0,0);
                        Console.WriteLine(new string('/', 10) + "   You lose   " + new string('/', 10));
                        Console.ReadLine();
                    }

                    // падение фигуры
                    FaillingFigure();

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

                    if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow)
                    {
                        var _pos = new Point(_figure.Position.X, _figure.Position.Y);
                        _pos.X--; // смещение фигуры влево

                        if (_field.CheckLimits(_figure, _pos) && _field.CheckCollision(_figure, _pos))
                        {
                            _figure.Position = _pos; // применяем новую позицию
                        }
                    }
                    else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow)
                    {
                        var _pos = new Point(_figure.Position.X, _figure.Position.Y);
                        _pos.X++; // смещение фигуры вправо

                        if (_field.CheckLimits(_figure, _pos) && _field.CheckCollision(_figure, _pos))
                        {
                            _figure.Position = _pos; // применяем новую позицию
                        }
                    }
                    else if (key == ConsoleKey.R || key == ConsoleKey.UpArrow)
                    {
                        // добавляем повороты
                        // нужное количество поворотов применится при отрисовке фигруы
                        _figure.RotateLength++;
                    }
                    else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow) 
                    {
                        // падение фигуры
                        FaillingFigure();
                    }
                }
            } while (true);
        }
        
        static IFigure TakeRandom()
        {
            var random = new Random();
            var figures = new []
            {
                new Teewee()            as IFigure, 
                new Hero()              as IFigure,
                new Square()            as IFigure,
                new OrangeRicky()       as IFigure, 
                new BlueRicky()         as IFigure,
                new PhodeIslandLeft()   as IFigure, 
                new PhodeIslandRight()  as IFigure, 
            };

            var figure = figures[random.Next(figures.Length)].Clone() as IFigure;
            
            figure.Position = new Point(4, 0);        // позиция - по центру игрового поля
            figure.RotateLength = random.Next(2);  // рандомный поворот
            
            return figure;
        }

        static void DrawStats()
        {
            Console.SetCursorPosition(7, 5);
            Console.WriteLine(" HovyTetris  \t {0} ", "0.17");
            Console.SetCursorPosition(7, 6);
            Console.WriteLine(" Cur. Speed  \t {0}   ", Speed + 1);
            Console.SetCursorPosition(7, 7);
            Console.WriteLine(" Burn Lines  \t {0}   ", Lines);
            Console.SetCursorPosition(7, 15);
            Console.WriteLine(".Game Design by");
            Console.SetCursorPosition(7, 16);
            Console.WriteLine("Alexey Pajitnov");
        }

        static void FaillingFigure()
        {
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
        }

        static void DrawNext()
        {
            (int x, int y) sizeFigure = (_nextFigure.Data[0].Length, _nextFigure.Data.Length);
            (int x, int y) sizeBar = (11, 5);
            (int x, int y) offset = (sizeBar.x / 2 - sizeFigure.x / 2, 
                                     sizeBar.y / 2 - sizeFigure.y / 2);

            for (int y = 0; y < sizeBar.y; y++)
            {
                Console.SetCursorPosition(_field.GlobalOffsetX + _field.Sizes.X + 2, 5 + y);
                
                for (int x = 0; x < sizeBar.x; x++)
                {
                    int xpos = x - offset.x;
                    int ypos = y - offset.y;
                    
                    Console.Write(xpos >= 0 && xpos < sizeFigure.x && 
                                  ypos >= 0 && ypos < sizeFigure.y ? _nextFigure.Data[ypos][xpos] : ' ');
                }
            }
        }
    }
}