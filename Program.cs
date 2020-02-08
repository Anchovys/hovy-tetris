using System;

namespace Tetris
{
    class Program
    {
        static void Main(string[] args)
        {
            // какие-то первоначальные настройки
            Console.CursorVisible = false;

            int currentTick = 0;
            int maxTick = 25;
            int controlTick = 5;

            var GameF = new GameField();

            var rnd = new Random();
            var f = new Square() { Position = new Point(5, 1) } as IFigure; 

            do
            {
                System.Threading.Thread.Sleep(10);
                currentTick++;

                if (currentTick % maxTick == 0)
                {
                    currentTick = 0; // сброс таймера

                    // проверка столкновений (на 1 клетку ниже)
                    if (!GameF.CheckCollisions(f, new Point(f.Position.X, f.Position.Y + 1)))
                    {
                        GameF.PlaceFigure(f); // размещаем фигуру где она есть

                        // появление новой
                        f = new Square() { Position = new Point(5, 1) } as IFigure;
                    }
                    else // нет столкновений с препятствием
                    {
                        f.FallDown(); // продолжаем падать
                    }

                }

                if(currentTick % controlTick == 0)
                {
                    int fs = GameF.CheckLine(); // найти Id полностью заполненной линии
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
                            f.MoveLeft();
                            break;
                        case ConsoleKey.D: // перемещение вправо
                            f.MoveRight();
                            break;
                        case ConsoleKey.S: // ускоренное падение
                            f.FallDown();
                            break;
                    }
                }
            } while (true);
        }
    }
}
