using System;
using System.Linq;
using System.Collections.Generic;

public static class PathFinder
{
    public enum AlgorithmType
    {
        AStar,
        ThetaStar,
        JumpPointSearch
    };

    public enum NeighbourhoodType
    {
        VonNeumann,
        Moore
    };

    public enum HeuristicType
    {
        Manhattan,
        Chebyshev,
        DiagonalShortcut,
        Euclidian,
        ApproxEuclidian
    };

    public enum LineOfSightType
    {
        Bresenham,
        Intersection
    };

    public static AlgorithmType Algorithm = AlgorithmType.ThetaStar;
    public static NeighbourhoodType Neighbourhood = NeighbourhoodType.Moore;
    public static HeuristicType Heuristic = HeuristicType.ApproxEuclidian;
    public static LineOfSightType LineOfSight = LineOfSightType.Intersection;

    public static Path<Node> FindPath(Node origin, Node destination)
    {
        HashSet<Node> closed = new HashSet<Node>();
        PriorityQueue<double, Path<Node>>  queue = new PriorityQueue<double, Path<Node>>();

        queue.Enqueue(0, new Path<Node>(origin));

        while (!queue.IsEmpty)
        {
            Path<Node> p = queue.Dequeue();

            if (closed.Contains(p.LastStep))
                continue;

            if (p.LastStep.Equals(destination))
                return p;

            closed.Add(p.LastStep);

            Node previous = null;
            if (p.PreviousSteps != null)
                previous = p.PreviousSteps.LastStep;

            foreach (Node n in GetNeighbours(p.LastStep, previous))
            {
                Path<Node> newPath = null;
                switch (Algorithm)
                {
                    case AlgorithmType.AStar:
                        newPath = p.AddStep(n, n.Cost + DistanceEstimate(p.LastStep, n));
                        break;

                    case AlgorithmType.ThetaStar:
                        if (previous != null && HasLineOfSight(previous, n))
                            newPath = p.PreviousSteps.AddStep(n, n.Cost + p.TotalCost - p.PreviousSteps.TotalCost + DistanceEstimate(previous, n));
                        else
                            newPath = p.AddStep(n, n.Cost + DistanceEstimate(p.LastStep, n));
                        break;

                    case AlgorithmType.JumpPointSearch:
                        if (previous != null)
                        {
                            Node jumpPoint = Jump(destination, n, p.LastStep);
                            if (jumpPoint != null)
                            {
                                if (HasLineOfSight(previous, jumpPoint))
                                    newPath = p.PreviousSteps.AddStep(jumpPoint, p.TotalCost - p.PreviousSteps.TotalCost + DistanceEstimate(previous, jumpPoint));
                                else
                                    newPath = p.AddStep(jumpPoint, p.TotalCost + DistanceEstimate(p.LastStep, jumpPoint));
                            }
                        }
                        else
                            newPath = p.AddStep(n, DistanceEstimate(p.LastStep, n));
                        break;
                }

                if (newPath != null)
                    queue.Enqueue(newPath.TotalCost + DistanceEstimate(n, destination), newPath);
            }
        }
        return null;
    }

    private static IEnumerable<Node> GetNeighbours(Node node, Node previous = null)
    {
        List<Node> neighbours = new List<Node>();

        Grid grid = node.Grid;

        int x = node.X;
        int y = node.Y;

        if (Algorithm == AlgorithmType.JumpPointSearch && previous != null)
        {
            int px = previous.X;
            int py = previous.Y;

            int dx = (x - px) / Math.Max(Math.Abs(x - px), 1);
            int dy = (y - py) / Math.Max(Math.Abs(y - py), 1);

            if (dx != 0 && dy != 0)
            {
                if (grid[x, y + dy].Passable)
                    neighbours.Add(grid[x, y + dy]);

                if (grid[x + dx, y].Passable)
                    neighbours.Add(grid[x + dx, y]);

                if (grid[x, y + dy].Passable || grid[x + dx, y].Passable)
                    neighbours.Add(grid[x + dx, y + dy]);

                if (!grid[x - dx, y].Passable && grid[x, y + dy].Passable)
                    neighbours.Add(grid[x - dx, y + dy]);

                if (!grid[x, y - dy].Passable && grid[x + dx, y].Passable)
                    neighbours.Add(grid[x + dx, y - dy]);
            }
            else
            {
                if (dx == 0)
                {
                    if (grid[x, y + dy].Passable)
                    {
                        neighbours.Add(grid[x, y + dy]);

                        if (!grid[x + 1, y].Passable)
                            neighbours.Add(grid[x + 1, y + dy]);

                        if (!grid[x - 1, y].Passable)
                            neighbours.Add(grid[x - 1, y + dy]);
                    }
                }
                else
                {
                    if (grid[x + dx, y].Passable)
                    {
                        neighbours.Add(grid[x + dx, y]);

                        if (!grid[x, y + 1].Passable)
                            neighbours.Add(grid[x + dx, y + 1]);

                        if (!grid[x, y - 1].Passable)
                            neighbours.Add(grid[x + dx, y - 1]);
                    }
                }
            }
        }
        else
        {
            switch (Neighbourhood)
            {
                case NeighbourhoodType.VonNeumann:
                    neighbours.Add(grid[x, y + 1]);
                    neighbours.Add(grid[x + 1, y]);
                    neighbours.Add(grid[x, y - 1]);
                    neighbours.Add(grid[x - 1, y]);
                    break;

                case NeighbourhoodType.Moore:
                    neighbours.Add(grid[x, y + 1]);
                    neighbours.Add(grid[x + 1, y]);
                    neighbours.Add(grid[x, y - 1]);
                    neighbours.Add(grid[x - 1, y]);
                    neighbours.Add(grid[x + 1, y + 1]);
                    neighbours.Add(grid[x - 1, y - 1]);
                    neighbours.Add(grid[x + 1, y - 1]);
                    neighbours.Add(grid[x - 1, y + 1]);
                    break;
            }
        }
        return neighbours.Where(n => n.Passable);
    }

    private static double DistanceEstimate(Node a, Node b)
    {
        float dx = Math.Abs(b.X - a.X);
        float dy = Math.Abs(b.Y - a.Y);

        switch (Heuristic)
        {
            default:
            case HeuristicType.Manhattan:
                return dx + dy;

            case HeuristicType.Chebyshev:
                return Math.Max(dx, dy);

            case HeuristicType.DiagonalShortcut:
                return (dx + dy) - 0.586f * Math.Min(dx, dy);

            case HeuristicType.Euclidian:
                return Math.Sqrt(dx * dx + dy * dy);

            case HeuristicType.ApproxEuclidian:
                int min = (int)Math.Min(dx, dy);
                int max = (int)Math.Max(dx, dy);
                int approx = (max * 1007) + (min * 441);
                if (max < (min << 4)) approx -= (max * 40);
                return ((approx + 512) >> 10);
        }
    }

    private static bool HasLineOfSight(Node a, Node b)
    {
        Grid grid = a.Grid;
        int x0 = a.X, y0 = a.Y;
        int x1 = b.X, y1 = b.Y;
        int x, y, dx, dy, sx, sy, error;

        switch (LineOfSight)
        {
            default:
            case LineOfSightType.Bresenham:
                bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
                if (steep)
                {
                    Swap(ref x0, ref y0);
                    Swap(ref x1, ref y1);
                }
                if (x0 > x1)
                {
                    Swap(ref x0, ref x1);
                    Swap(ref y0, ref y1);
                }
                dx = x1 - x0;
                dy = Math.Abs(y1 - y0);
                error = dx / 2;
                sy = (y0 < y1) ? 1 : -1;
                y = y0;
                for (x = x0; x <= x1; x++)
                {
                    if (!grid[steep ? y : x, steep ? x : y].Passable)
                        return false;

                    error = error - dy;
                    if (error < 0)
                    {
                        y += sy;
                        error += dx;
                    }
                }
                return true;

            case LineOfSightType.Intersection:
                dx = Math.Abs(x1 - x0);
                dy = Math.Abs(y1 - y0);
                x = x0;
                y = y0;
                int n = 1 + dx + dy;
                sx = (x1 > x0) ? 1 : -1;
                sy = (y1 > y0) ? 1 : -1;
                error = dx - dy;
                dx *= 2;
                dy *= 2;
                for (; n > 0; n--)
                {
                    if (!grid[x, y].Passable)
                        return false;

                    if (error > 0)
                    {
                        x += sx;
                        error -= dy;
                    }
                    else
                    {
                        y += sy;
                        error += dx;
                    }
                }
                return true;
        }
    }

    private static void Swap<T>(ref T a, ref T b)
    {
        T temp;
        temp = a;
        a = b;
        b = temp;
    }

    private static Node Jump(Node destination, Node current, Node previous)
    {
        Grid grid = destination.Grid;
        int x = current.X;
        int y = current.Y;
        int dx = x - previous.X;
        int dy = y - previous.Y;

        if (!grid[x, y].Passable)
            return null;

        if (grid[x, y].Equals(destination))
            return destination;

        if (dx != 0 && dy != 0)
        {
            if ((grid[x - dx, y + dy].Passable && !grid[x - dx, y].Passable) || (grid[x + dx, y - dy].Passable && !grid[x, y - dy].Passable))
                return grid[x, y];

            if (Jump(destination, grid[x + dx, y], current) != null || Jump(destination, grid[x, y + dy], current) != null)
                return grid[x, y];
        }
        else
        {
            if (dx != 0)
            {
                if ((grid[x + dx, y + 1].Passable && !grid[x, y + 1].Passable) || (grid[x + dx, y - 1].Passable && !grid[x, y - 1].Passable))
                    return grid[x, y];
            }
            else
            {
                if ((grid[x + 1, y + dy].Passable && !grid[x + 1, y].Passable) || (grid[x - 1, y + dy].Passable && !grid[x - 1, y].Passable))
                    return grid[x, y];
            }
        }

        if (grid[x + dx, y].Passable || grid[x, y + dy].Passable)
            return Jump(destination, grid[x + dx, y + dy], current);
        else
            return null;
    }
}
