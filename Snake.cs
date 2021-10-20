using System;
using System.Collections.Generic;

namespace tak
{
    public partial class Game
    {
        class Snake
        {
            private List<cell> snake = new List<cell>();
            private bool addCell = false;

            public Snake()
            {
                snake.Add(new cell { position = (5, 10) });
            }

            public void AddCell((int X, int Y) position)
            {
                snake.Add(new cell { position = position });
            }

            public void AddCell()
            {
                addCell = true;
            }

            public IEnumerable<(int X, int Y)> GetCellsPositions()
            {
                foreach (var item in snake)
                {
                    yield return item.position;
                }
            }

            public void UpdateSnake(Direction direction)
            {
                (int X, int Y) temp = (0, 0);
                switch (direction)
                {
                    case Direction.Up:
                        temp = (-1, 0);
                        break;
                    case Direction.Down:
                        temp = (1, 0);
                        break;
                    case Direction.Left:
                        temp = (0, -1);
                        break;
                    case Direction.Right:
                        temp = (0, 1);
                        break;
                    default:
                        Console.WriteLine("bruh");
                        Environment.Exit(1);
                        break;
                }

                var newPos = (snake[0].position.X + temp.X, snake[0].position.Y + temp.Y);

                if (newPos.Item1 < 0)
                    newPos.Item1 = areaSize.X - 1;

                if (newPos.Item2 < 0)
                    newPos.Item2 = areaSize.Y - 1;

                if (newPos.Item1 >= areaSize.X)
                    newPos.Item1 = 0;

                if (newPos.Item2 >= areaSize.Y)
                    newPos.Item2 = 0;

                snake.Insert(0, new cell { position = newPos });

                if (!addCell)
                {
                    snake.RemoveAt(snake.Count - 1);
                }

                addCell = false;
            }

            struct cell
            {
                public (int X, int Y) position;
            }
        }
    }
}