using System;
using System.Threading;

namespace GameOfLife
{
    class Program
    {
        static void Main(string[] args)
        {
            //*******  to John Conway R.I.P.  ********//
            // rules
            /*  Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                Any live cell with two or three live neighbours lives on to the next generation.
                Any live cell with more than three live neighbours dies, as if by overpopulation.
                Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction. */

            // Dimension
            int width = 40;
            int height = 20;

            bool[,] world = new bool[width, height];

            Random random = new Random();
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    world[j, i] = random.Next(2) == 0;

            while (true)
            {
                Draw(world);
                GenNextGen(ref world);
                Thread.Sleep(1000);
            }
        }

        static void GenNextGen(ref bool[,] world)
        {
            int width = world.GetLength(0);
            int height = world.GetLength(1);
            bool[,] newWorld = new bool[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //cell
                    int aliveNeighbours = 0;

                    //neighbours
                    for (int i = y - 1; i <= y + 1; i++)
                    {
                        for (int j = x - 1; j <= x + 1; j++)
                        {
                            if (!world[(j + width) % width, (i + height) % height]) continue;
                            if (y != i || x != j)
                                aliveNeighbours++;
                        }
                    }

                    newWorld[x, y] = world[x, y] ? aliveNeighbours >= 2 && aliveNeighbours <= 3 : aliveNeighbours == 3;
                }
            }

            world = newWorld;
        }

        static void Draw(bool[,] world)
        {
            Console.SetCursorPosition(0, 0);
            string gen = "";
            for (int y = 0; y < world.GetLength(1); y++)
            {
                for (int x = 0; x < world.GetLength(0); x++)
                {
                    gen += world[x, y] ? "■" : " ";
                }
                gen += "\n";
            }
            Console.Write(gen);
        }
    }
}
