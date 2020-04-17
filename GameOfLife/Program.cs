using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife
{
    static class Program
    {
        static int cycle = 0;
        private static int maxTasks = 5;
        static void Main(string[] args)
        {
            //*******  to John Conway R.I.P.  ********//
            // rules
            /*  Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                Any live cell with two or three live neighbours lives on to the next generation.
                Any live cell with more than three live neighbours dies, as if by overpopulation.
                Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction. */

            // Dimension
            int width = 50;
            int height = 20;

            bool[,] world = new bool[width, height];

            Random random = new Random();
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    world[j, i] = random.Next(5) == 0;

            while (true)
            {
                Draw(world);
                world = GenNextGen(world);
                Thread.Sleep(200);
            }
        }

        static bool[,] GenNextGen(bool[,] world)
        {
            int width = world.GetLength(0);
            int height = world.GetLength(1);
            bool[,] newWorld = new bool[width, height];

            int taskNumbers = GreatestDivisableNumber(height,
                maxTasks > height ? height : maxTasks);
            Task[] tasks = new Task[taskNumbers];

            for (int o = 0; o < taskNumbers; o++)
            {
                int o1 = o;
                tasks[o] = Task.Factory.StartNew(() =>
                {
                    int todiv = height / taskNumbers;
                    for (int y = (o1 != 0 ? (todiv * o1) : 0); y < (todiv * (o1 + 1)); y++)
                        for (int x = 0; x < width; x++)
                        {
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

                    //Console.WriteLine("Task:"+o1);
                });
            }

            Task.WaitAll(tasks);
            //Console.WriteLine("Boom + " + taskNumbers);
            return newWorld;

        }

        static int GreatestDivisableNumber(int init, int max = 20)
        {
            int number = 0;
            for (int i = 1; i <= max; i++)
            {
                if (init % i == 0) number = i;
            }

            return number;
        }

        static void Draw(bool[,] world)
        {
            Console.SetCursorPosition(0, 0);
            int width = world.GetLength(0);
            int height = world.GetLength(1);
            int taskNumbers = GreatestDivisableNumber(height, maxTasks > height ? height : maxTasks);

            Task<string>[] tasks = new Task<string>[taskNumbers];

            for (int i = 0; i < taskNumbers; i++)
            {
                int o1 = i;
                int todiv = height / taskNumbers;
                tasks[i] = Task<string>.Factory.StartNew(() =>
                {
                    string gen = "";
                    for (int y = (o1 != 0 ? (todiv * o1) : 0); y < todiv * (o1 + 1); y++)
                    {
                        for (int x = 0; x < width; x++)
                            gen += world[x, y] ? "■" : " ";

                        gen += "\n";
                    }

                    return gen;
                });
            }

            Task.WaitAll(tasks);

            foreach (var task in tasks)
            {
                Console.Write(task.Result);
            }

            Console.WriteLine("Cycle: " + cycle++);
        }

        static void Write(bool[,] world)
        {
            Console.SetCursorPosition(0, 0);
            int width = world.GetLength(0);
            int height = world.GetLength(1);
            int taskNumbers = GreatestDivisableNumber(height, maxTasks > height ? height : maxTasks);

            Task<string>[] tasks = new Task<string>[taskNumbers];

            for (int i = 0; i < taskNumbers; i++)
            {
                int o1 = i;
                int todiv = height / taskNumbers;
                tasks[i] = Task<string>.Factory.StartNew(() =>
                {
                    string gen = "";
                    for (int y = (o1 != 0 ? (todiv * o1) : 0); y < todiv * (o1 + 1); y++)
                    {
                        for (int x = 0; x < width; x++)
                            gen += world[x, y] ? "■" : " ";
                        //Console.Write(world[x, y] ? "■" : " ");

                        gen += "\n";
                        //Console.WriteLine();
                    }

                    return gen;
                });
            }

            Task.WaitAll(tasks);

            using (var file = File.OpenWrite("generated.txt"))
            {
                foreach (var task in tasks)
                {
                    file.Write(Encoding.ASCII.GetBytes(task.Result));
                }
            }
        }
    }
}
