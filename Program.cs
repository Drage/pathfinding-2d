using System;
using System.Linq;
using System.Diagnostics;

namespace Pathfinding2D
{
    class Program
    {
        static int[,] maze = new int[,]
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1 },
            { 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1 },
            { 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1 },
            { 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };

        static int scale = 4;

        static void Main(string[] args)
        {
            int width = maze.GetLength(1) * scale;
            int height = maze.GetLength(0) * scale;

            Grid grid = new Grid(width, height);

            Console.WriteLine("{0} x {1} = {2} cells", width, height, width * height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (maze[y / scale, x / scale] == 1)
                        grid[x, y].Passable = false;
                }
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();

            PathFinder.Algorithm = PathFinder.AlgorithmType.JumpPointSearch;
            PathFinder.Neighbourhood = PathFinder.NeighbourhoodType.Moore;
            PathFinder.Heuristic = PathFinder.HeuristicType.ApproxEuclidian;
            PathFinder.LineOfSight = PathFinder.LineOfSightType.Intersection;
 
            Path<Node> path = PathFinder.FindPath(grid[0 * scale, 3 * scale], grid[16 * scale, 11 * scale]);

            Console.WriteLine("path calculation time = {0} ms", timer.ElapsedMilliseconds);

            if (path == null)
                Console.WriteLine("No path found");
            else
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        if (path.Contains(grid[x, y]))
                            Console.Write(path.Reverse().ToList().FindIndex(n => n == grid[x, y]) % 10);
                        else
                            Console.Write(grid[x, y].Passable ? " " : "#");
                    }
                    Console.WriteLine();
                }
            }
            Console.ReadKey();
        }
    }
}
