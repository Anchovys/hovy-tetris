using System;

namespace Tetris
{
    class Program
    {
        static Random rnd = new Random();

        static IFigure GetFigure()
        {
            switch (rnd.Next(2))
            {
                case 0:
                    return new Square() { Position = new Point(5, 0) } as IFigure;
                case 1:
                    return new Teewee() { Position = new Point(5, 0) } as IFigure;
                default:
                    return null;
            }
        }

        static void Main(string[] args)
        {
            // какие-то первоначальные настройки
            Console.CursorVisible = false;

            int currentTick = 0;
            int maxTick = 10;
            int controlTick = 5;

            var GameF = new GameField();

            var f = GetFigure();

            do
            {
                System.Threading.Thread.Sleep(10);
                currentTick++;

                if (currentTick % maxTick == 0)
                {
                    Console.WriteLine("Fine");

                    // проверка столкновений (на 1 клетку ниже)
                    if (f.Position.Y + f.Data.Length >= GameF.Sizes.Y ||
                        !GameF.CheckCollisions(f, new Point(f.Position.X, f.Position.Y + 1)))
                    {
                        GameF.PlaceFigure(f); // размещаем фигуру где она есть

                        // появление новой
                        f = GetFigure();

                        if (GameF.Lines[0].Contains('#'))
                        {
                            GameF.FillEmpty(); // game over
                        }

                    }
                    else // нет столкновений с препятствием
                    {
                        f.FallDown(); // продолжаем падать
                    }

                    currentTick = 0; // сброс таймера

                }

                if (currentTick % controlTick == 0)
                {
                    int fs = GameF.CheckLine('#'); // найти Id полностью заполненной линии
                    if (fs != -1) // она есть
                    {
                        GameF.DeleteLine(fs); // удалить ее
                    }

                    // отрисовка всего экрана
                    GameF.DrawScreen();

                    Console.WriteLine("1234567890");

                    // отрисовка падающей фигуры (отдельно)
                    GameF.DrawFigure(f);

                    // никакую кнопку не обрабатываем
                    if (!Console.KeyAvailable)
                    {
                        continue; // выходим
                    }

                    // в зависимости от того, какую кнопку зажали
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.A: // перемещение влево

                            if (!GameF.CheckLimits(f, new Point(f.Position.X - 1, f.Position.Y)))
                            {
                                continue;
                            }

                            // проверка столкновений (на 1 клетку ниже)
                            if (GameF.CheckCollisions(f, new Point(f.Position.X - 1, f.Position.Y)))
                            {
                                f.MoveLeft(); // продолжаем падать
                            }
                            break;
                        case ConsoleKey.D: // перемещение вправо

                            
                            if(!GameF.CheckLimits(f, new Point(f.Position.X + 1, f.Position.Y)))
                            {
                                continue;
                            }

                            if (GameF.CheckCollisions(f, new Point(f.Position.X + 1, f.Position.Y)))
                            {
                                f.MoveRight(); // продолжаем падать
                            }
                            
                            break;
                        case ConsoleKey.S: // ускоренное падение
                           // f.FallDown();
                            break;

                        case ConsoleKey.R:
                            f.Rotate();
                            break;
                             
                    }
                }
            } while (true);
        }
    }
}
