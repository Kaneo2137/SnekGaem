using System;

namespace tak
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            while (!game.isGameOver)
            {
                game.Update();
                game.Render();
            }
        }

    }
}
