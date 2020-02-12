using System;
using System.Threading;

namespace Tetris
{
    public class Program
    {
        private static GameField _field = new GameField(Console.BufferWidth);
        
        private static IFigure _figure = TakeRandom();
        private static IFigure _nextFigure = TakeRandom();
        
        private static int _lines;
        private static int _speed; 

        private static bool _pause = false;
        
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
            Console.WriteLine(" Cur. Speed  \t {0}   ", _speed + 1);
            Console.SetCursorPosition(7, 7);
            Console.WriteLine(" Burn Lines  \t {0}   ", _lines);
            Console.SetCursorPosition(7, 15);
            Console.WriteLine(".Game Design by");
            Console.SetCursorPosition(7, 16);
            Console.WriteLine("Alexey Pajitnov");
        }

        static void FaillingFigure()
        {
            var pos = new Point(_figure.Position.X, _figure.Position.Y);
            pos.Y++; // смещение фигуры вниз
                        
            if (_field.CheckLimits(_figure, pos) && _field.CheckCollision(_figure, pos))
            {
                _figure.Position = pos; // смещаем фигуру вниз
            }
            else
            {
                _field.PlaceFigure(_figure);
                _figure = _nextFigure;
                _nextFigure = TakeRandom();
                
                DrawNext();
            }
        }

        static void Restart()
        {
            // переиницилизируем игровое поле
            _field = new GameField(Console.BufferWidth);
            
            // новые фигуры для всего
            _figure = TakeRandom(); 
            _nextFigure = TakeRandom();
            
            _lines = 0; // сброс очков
            _speed = 0; // сброс скорости
            
            DrawNext();   // рисуем след. фигуру
        }

        static void Main()
        {
            Console.CursorVisible = false;

            Console.WriteLine("HovyTetris READ THIS INSTRUCTION!" +
                              "\n\nControls : " +
                              "\n\t[ P / enter ]\t\t - set/unset pause" +
                              "\n\t[ R ]\t\t\t - restart game" +
                              "\n\t[ E / up arrow ]\t\t - rotate (clockwise)" +
                              "\n\t[ A / left arrow ]\t - move left" +
                              "\n\t[ D / right arrow ]\t - move right" +
                              "\n\t[ S / down arrow ]\t - move down (falling)" +
                              "\nBurn 2 lines + 1 speed (max speed - 15)" +
                              "\n\n\tTap [enter] for start!");
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
                Thread.Sleep(20 - _speed);
                currentTick++;

                if (currentTick % renderTick == 0)
                {
                    currentTick = 0; // сброс таймера

                    if (_pause)
                    {
                        continue;
                    }

                    // попытка сжечь линию
                    int fillId = _field.FindFullLine();
                    if (fillId != -1) // есть такая
                    {
                        _field.DeleteLine(fillId); // сжигаем
                        _lines++; // записываем в очки

                        // тут же изменяем скорость игры
                        if (_lines % 2 == 0 && _speed < 15)
                        {
                            _speed++;
                        }
                    }

                    // проверка на провал
                    if (_field.CheckContainsLine(0, '#'))
                    {
                        Console.SetCursorPosition(0,0);
                        Console.WriteLine("YOU LOSE! Press any key for restart!");
                        
                        Console.ReadKey(); // просим нажать любую кнопку
                        Restart(); // рестарт игры
                    }

                    // падение фигуры
                    FaillingFigure();

                    // отрисовка статистики и т.д
                    DrawStats();
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

                    //чтобы не писать символом на поле
                    Console.SetCursorPosition(0,0);
                    
                    // в зависимости от того, какую кнопку зажали
                    ConsoleKey key = Console.ReadKey().Key;

                    if (key == ConsoleKey.P || key == ConsoleKey.Enter)
                    {
                        _pause = !_pause;
                    }
                    
                    if (_pause)
                    {
                        continue;
                    }

                    if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow)
                    {
                        var pos = new Point(_figure.Position.X, _figure.Position.Y);
                        pos.X--; // смещение фигуры влево

                        if (_field.CheckLimits(_figure, pos) && _field.CheckCollision(_figure, pos))
                        {
                            _figure.Position = pos; // применяем новую позицию
                        }
                    }
                    else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow)
                    {
                        var pos = new Point(_figure.Position.X, _figure.Position.Y);
                        pos.X++; // смещение фигуры вправо

                        if (_field.CheckLimits(_figure, pos) && _field.CheckCollision(_figure, pos))
                        {
                            _figure.Position = pos; // применяем новую позицию
                        }
                    }
                    else if (key == ConsoleKey.E || key == ConsoleKey.UpArrow)
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
                    else if (key == ConsoleKey.R)
                    {
                        // рестартуем
                        Restart();
                    }
                }
            } while (true);
        }
    }
}