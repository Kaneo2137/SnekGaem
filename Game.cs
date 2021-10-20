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
        private int[,] gameArea;

        // W couple można nazywać poszczególne elementy, co mega ułatwia pracę
        private static (int X, int Y) areaSize = (10, 20);

        private Snake snake = new Snake();
        private bool isFruitPresent = false;
        private (int X, int Y) fruitLocation = (0, 0);
        private Direction direction = Direction.Up;
        private static Mutex directionMutex = new Mutex();

        // Reminder dla mnie w przyszłości. Jedna sekunda jest tutaj dzielona na ilość
        // klatek jakie chcemy otrzymać. 1000 ms dzielone na 2 klatki daje nam
        // frametime na poziomie 500 ms.
        private int tickrate = 1000 / 3;

        private Thread movement;
        private Random rnd = new Random();

        public Game()
        {
            isGameOver = false;

            gameArea = new int[areaSize.X, areaSize.Y];

            movement = new Thread(MovementThread);
            movement.Start();
        }

        // TODO: Dobra, trzeba ogarnąć losowe wrzucanie owocków                       - done
        //       System sterowania (oddzielny wątek wychwytujący kliki i jakiś lock?) - done
        //       System kolizji -> Dotknięcie ściany (przejście na drugą stronę?),    - done
        //                  dotykanie owocków oraz dotykanie samego siebie przez węża - partially done
        //       Jakieś punkty by się może przydały?

        public void Update()
        {
            directionMutex.WaitOne();

            snake.UpdateSnake(direction);

            directionMutex.ReleaseMutex();

            if (snake.GetCellsPositions().First() == fruitLocation)
            {
                snake.AddCell();
                isFruitPresent = false;
            }

            // zero whole array
            for (int i = 0; i < areaSize.X; i++)
                for (int j = 0; j < areaSize.Y; j++)
                    gameArea[i, j] = 0;

            // change certain fields to make a snake
            foreach (var item in snake.GetCellsPositions())
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

            Thread.Sleep(tickrate);
        }

        public void Render()
        {
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
                            Console.WriteLine("bruh");
                            System.Environment.Exit(1);
                            break;
                    }
                }
                Console.WriteLine(line);

                line = new char[areaSize.Y];
            }
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