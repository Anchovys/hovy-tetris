using System;

namespace TetrisBool
{
    public class Program
    {
        static GameField _field = new GameField();
        private static IFigure _figure = new Teewee(new Point(4, 0));
        
        static void Main()
        {
            _field.FillField(false);
            
            int currentTick = 0;
            int maxTick = 10;
            int controlTick = 5;
            
            do
            {
                System.Threading.Thread.Sleep(10);
                currentTick++;

                if (currentTick % maxTick == 0)
                {
                    Console.WriteLine("Fine");

                    var _pos = new Point(_figure.Position.X, _figure.Position.Y);
                    _pos.Y++; // смещение фигуры влево
                        
                    if (_figure.Position.Y >= 15 || !_field.CheckCollision(_figure, _pos))
                    {
                        _field.PlaceFigure(_figure);
                        _figure = new Teewee(new Point(4, 0));
                    }
                    else
                    {
                        _figure.Position = _pos;    
                    }

                    currentTick = 0; // сброс таймера
                }

                if (currentTick % controlTick == 0)
                {
                    // отрисовка всего экрана
                    _field.DrawScreen();

                    Console.WriteLine("1234567890");

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
                            _figure.Position = _pos;  // применяем новую позицию
                        }
                    }
                }
            } while (true);
        }
    }
}