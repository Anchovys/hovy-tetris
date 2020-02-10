using System;

namespace TetrisBool
{
    public class GameField
    {
        public bool[,] MapData = new bool[10, 20]; 
        
        public void FillField(bool fillValue = false) {

            for (var j = 0; j < MapData.GetLength(1); j++)
            {
                for (var i = 0; i < MapData.GetLength(0); i++)
                {
                    MapData[i, j] = fillValue;
                }
            }
        }

        public void DrawScreen()
        {
            Console.Clear();
            
            for (var j = 0; j < MapData.GetLength(1); j++)
            {
                for (var i = 0; i < MapData.GetLength(0); i++)
                {
                    Console.Write(MapData[i, j] ? '#' : ' ');
                }

                Console.Write('\n');
            }
        }

        public void DrawFigure(IFigure figure)
        {
            var pos = new Point(figure.Position.X, figure.Position.Y);
            var data = figure.Data;        // TODO : переворот
            var cursorPos = new Point(Console.CursorTop, Console.CursorLeft);

            for (var j = 0; j < data.GetLength(1); j++)
            {
                for (var i = 0; i < data.GetLength(0); i++)
                {
                    Console.SetCursorPosition(pos.X + i, pos.Y + j);

                    // попытка "закрасить" что-то пробелом пресекается
                    if (MapData[pos.X + i, pos.Y + j] && !data[i, j])
                    {
                        return;
                    }

                    Console.Write(data[i, j] ? '#' : ' ');
                }
            }
            Console.SetCursorPosition(cursorPos.X, cursorPos.Y);
        }

        public void PlaceFigure(IFigure figure)
        {
            var pos = new Point(figure.Position.X, figure.Position.Y);
            var data = figure.Data; // TODO : переворот
            
            for (var j = 0; j < data.GetLength(1); j++)
            {
                for (var i = 0; i < data.GetLength(0); i++)
                {
                    // попытка "закрасить" что-то пробелом пресекается
                    if (MapData[pos.X + i, pos.Y + j] && !data[i, j])
                    {
                        return;
                    }
                    
                    // помещаем на карте
                    MapData[pos.X + i, pos.Y + j] = data[i, j];
                }
            }
        }

        public bool CheckLimits(IFigure figure, Point customPos = null)
        {
            var pos = customPos == null ? figure.Position : customPos;
            
            return pos.X >= 0 && pos.X + figure.Data.GetLength(0) <= MapData.GetLength(0) &&
                   pos.Y >= 0 && pos.Y + figure.Data.GetLength(1) <= MapData.GetLength(1);
        }

        public bool CheckCollision(IFigure figure, Point customPos = null)
        {
            var pos = customPos == null ? figure.Position : customPos;
            var data = figure.Data; // TODO : переворот
            
            for (var j = 0; j < data.GetLength(1); j++)
            {
                for (var i = 0; i < data.GetLength(0); i++)
                {
                    // попытка "закрасить" что-то пробелом пресекается
                    if (MapData[pos.X + i, pos.Y + j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}