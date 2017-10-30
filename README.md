# pathfinding-2d
Efficient grid-based pathfinding, including A*, Theta*, and JPS implementations

## Algorithms
- A*: Standard A* algorithm
- Theta*: Variant of A* allowing diagonal paths that cross unobstructed grid cells
- JPS (Jump Point Search): Very efficient algorithm for generating paths across uniform cost grids

A* vs Theta* path computation
![A* vs Theta*](http://aigamedev.com/static/tutorials/aap-pathcompare2D.png)

Jump Point Search path computation
![JPS](http://runedegroot.com/wp-content/uploads/sites/2/2015/11/JPS-691x249.png)

## Neighbourhood Types
- Von Neumann: 8 adjacent neighbours (allows diagonal movement)
- Moore: 4 adjacent neigbours

## Line-of-sight Types
- Bresenham: Fast line drawing approximation
- Intersection: Considers all cells intersecting line for obstructions

## Heuristic Types
- Manhattan
- Chebyshev
- DiagonalShortcut
- Euclidian
- ApproxEuclidian (approximation without using sqrt)

## Usage
```
// Create a grid of cells
Grid grid = new Grid(width, height);

// Set whether cells are passable (may be traversed)
for (int x = 0; x < width; x++)
{
    for (int y = 0; y < height; y++)
    {
        if (maze[y, x] == 1)
            grid[x, y].Passable = false;
    }
}

// Set path finder configuration
PathFinder.Algorithm = PathFinder.AlgorithmType.JumpPointSearch;
PathFinder.Neighbourhood = PathFinder.NeighbourhoodType.Moore;
PathFinder.Heuristic = PathFinder.HeuristicType.ApproxEuclidian;
PathFinder.LineOfSight = PathFinder.LineOfSightType.Intersection;

// Search for shortest path between two cells in the grid
Path<Node> path = PathFinder.FindPath(grid[0, 3], grid[16, 11]);

// Print result
path.Reverse().ToList().ForEach(n => Console.WriteLine("[{0}, {1}]", n.X, n.Y));
```
