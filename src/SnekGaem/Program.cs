using System;

namespace SnekGaem
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var game = new Game();
                while (!game.isGameOver)
                {
                    game.Update();
                    game.Render();
                }

                Console.WriteLine("Kliknij R, aby zresetować grę\nNaciśnij cokolwiek, żeby wyłączyć grę.");

                var key = Console.ReadKey(true).KeyChar;
                if (key == 'r' || key == 'R')
                    continue;
                else
                    break;
            }
        }

    }
}
