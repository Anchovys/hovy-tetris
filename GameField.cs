using System;
using System.Collections.Generic;

namespace TetrisBool
{
    public class GameField
    {
        private bool[,] MapData = new bool[10, 20]; 
        
        public void FillField(bool fillValue = false) {

            for (var y = 0; y < MapData.GetLength(1); y++)
            {
                for (var x = 0; x < MapData.GetLength(0); x++)
                {
                    MapData[x, y] = fillValue;
                }
            }
        }

        public void DrawScreen()
        {
            Console.Clear();
            
            for (var y = 0; y < MapData.GetLength(1); y++)
            {
                for (var x = 0; x < MapData.GetLength(0); x++)
                {
                    Console.Write(MapData[x, y] ? '#' : ' ');
                }

                Console.Write('\n');
            }
        }
        
        public void DrawFigure(IFigure figure)
        {
            var pos = new Point(figure.Position.X, figure.Position.Y);
            var data = figure.Data;        // TODO : переворот
            var cursorPos = new Point(Console.CursorTop, Console.CursorLeft);

            for (var y = 0; y < data.GetLength(1); y++)
            {
                for (var x = 0; x < data.GetLength(0); x++)
                {
                    Console.SetCursorPosition(pos.X + x, pos.Y + y);

                    // попытка "закрасить" что-то пробелом пресекается
                    if (MapData[pos.X + x, pos.Y + y] && !data[x, y])
                    {
                        return;
                    }

                    Console.Write(data[x, y] ? '#' : ' ');
                }
            }
            Console.SetCursorPosition(cursorPos.X, cursorPos.Y);
        }

        public void PlaceFigure(IFigure figure)
        {
            var pos = new Point(figure.Position.X, figure.Position.Y);
            var data = figure.Data; // TODO : переворот
            
            for (var y = 0; y < data.GetLength(1); y++) // Y axis
            {
                for (var x = 0; x < data.GetLength(0); x++) // X axis
                {
                    // попытка "закрасить" что-то пробелом пресекается
                    if (MapData[pos.X + x, pos.Y + y] && !data[x, y])
                    {
                        return;
                    }
                    
                    // помещаем на карте
                    MapData[pos.X + x, pos.Y + y] = data[x, y];
                }
            }
        }

        public bool CheckLimits(IFigure figure, Point customPos = null)
        {
            var pos = customPos == null ? figure.Position : customPos;
            
            return pos.X >= 0 && pos.X + figure.Data.GetLength(0) <= MapData.GetLength(0) &&
                   pos.Y >= 0 && pos.Y + figure.Data.GetLength(1) <= MapData.GetLength(1);
        }

        public void DeleteLine(int id)
        {
            if(id < 0 || id > MapData.GetLength(1))
            {
                return;
            }
            var t = new bool[id][];
            
            for (var y = 0; y < id; y++)
            {
                if (t[y] == null)
                {
                    t[y] = new bool[MapData.GetLength(0)];
                }

                for (var x = 0; x < MapData.GetLength(0); x++) // X axis
                {
                    t[y][x] = MapData[x, y];
                }
            }
            
            for (var y = 1; y <= id; y++)
            {
                for (var x = 0; x < MapData.GetLength(0); x++)
                {
                    MapData[x, y] = t[y-1][x];
                }
            }

            // заполняем верхнюю строку нулями
            FillLine(0, false);
        }

        public void FillLine(int id, bool value = false)
        {
            for (var x = 0; x < MapData.GetLength(0); x++)
            {
                MapData[x, id] = value;
            }
        }

        public int FindFullLine()
        {
            for (var y = 0; y < MapData.GetLength(1); y++)
            {
                if (CheckFullLine(y))
                {
                    return y;
                }
            }

            return -1;
        }

        public bool CheckFullLine(int id, bool all = true)
        {
            if(id < 0 || id > MapData.GetLength(1))
            {
                return false;
            }
            
            for (var x = 0; x < MapData.GetLength(0); x++)
            {
                if (all)
                {
                    if (!MapData[x, id])
                    {
                        return false;
                    }
                }
                else
                {
                    if (MapData[x, id])
                    {
                        return true;
                    }
                }
            }
            
            return all;

        }
        
        public bool CheckCollision(IFigure figure, Point customPos = null)
        {
            var pos = customPos == null ? figure.Position : customPos;
            var data = figure.Data; // TODO : переворот
            
            for (var y = 0; y < data.GetLength(1); y++)
            {
                for (var x = 0; x < data.GetLength(0); x++)
                {
                    // попытка "закрасить" что-то пробелом пресекается
                    if (MapData[pos.X + x, pos.Y + y])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}