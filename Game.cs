using System;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace tak
{
    public partial class Game
    {
        public bool isGameOver { get; private set; }

        // 0 - Pusta przestrzeń
        // 1 - Wonsz
        // 2 - Jagódka? Karma dla węża?

        private int[,] gameArea = new int[areaSize.X, areaSize.Y];
        private static (int X, int Y) areaSize = (10, 20);
        private int points = 0;

        private Snake snake = new Snake();
        private bool isFruitPresent = false;
        private (int X, int Y) fruitLocation = (0, 0);
        private Direction direction = Direction.Up;
        private static Mutex directionMutex = new Mutex();

        private int tickrate = 1000 / 3;
        private Thread movement;
        private Random rnd = new Random();

        public Game()
        {
            isGameOver = false;

            movement = new Thread(MovementThread);
            movement.Start();
        }

        public void Update()
        {
            directionMutex.WaitOne();
            snake.UpdateSnake(direction);
            directionMutex.ReleaseMutex();

            var snakeCellsPositions = snake.GetCellsPositions();

            if (snakeCellsPositions.Count() != snakeCellsPositions.Distinct().Count())
            {
                isGameOver = true;
                return;
            }

            if (snakeCellsPositions.First() == fruitLocation)
            {
                points++;
                snake.AddCell();
                isFruitPresent = false;
            }

            for (int i = 0; i < areaSize.X; i++)
                for (int j = 0; j < areaSize.Y; j++)
                    gameArea[i, j] = 0;

            foreach (var item in snakeCellsPositions)
            {
                gameArea[item.X, item.Y] = 1;
            }

            if (!isFruitPresent)
            {
                while (true)
                {
                    int x = rnd.Next(areaSize.X);
                    int y = rnd.Next(areaSize.Y);

                    if (gameArea[x, y] != 0)
                        continue;

                    gameArea[x, y] = 2;
                    fruitLocation = (x, y);

                    break;
                }

                isFruitPresent = true;
            }
            else
                gameArea[fruitLocation.X, fruitLocation.Y] = 2;
        }

        public void Render()
        {
            if (isGameOver)
                return;

            Console.Clear();

            char[] line = new char[areaSize.Y];

            for (int i = 0; i < areaSize.X; i++)
            {
                for (int j = 0; j < areaSize.Y; j++)
                {
                    switch (gameArea[i, j])
                    {
                        case 0:
                            line[j] = '.';
                            break;
                        case 1:
                            line[j] = 'X';
                            break;
                        case 2:
                            line[j] = 'O';
                            break;
                        default:
                            throw new Exception("Wrong value in the gameArea array");
                    }
                }
                Console.WriteLine(line);

                line = new char[areaSize.Y];
            }

            Console.WriteLine($"\nYour points: {points}");

            Thread.Sleep(tickrate);
        }

        private void MovementThread()
        {
            while (!isGameOver)
            {
                var key = Console.ReadKey(true).KeyChar;

                directionMutex.WaitOne();

                switch (key)
                {
                    case 'w':
                        direction = Direction.Up;
                        break;
                    case 's':
                        direction = Direction.Down;
                        break;
                    case 'a':
                        direction = Direction.Left;
                        break;
                    case 'd':
                        direction = Direction.Right;
                        break;
                    default:
                        break;
                }

                directionMutex.ReleaseMutex();
            }
        }

        enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }
    }
}