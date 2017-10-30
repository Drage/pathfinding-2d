public class Grid
{
	private Node[,] grid;

	public int Width { get; private set; }
	public int Height { get; private set; }

    public Grid(int width, int height)
	{
		this.Width = width;
		this.Height = height;

        grid = new Node[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Node node = new Node(this, x, y);
                grid[x, y] = node;
            }
        }
	}
	
	public Node this[int x, int y]
	{
		get
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                Node outOfBounds = new Node(this, x, y);
                outOfBounds.Passable = false;
                return outOfBounds;
            }
            return grid[x, y];
        }
	    set
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;

            grid[x, y] = value;
        }
	}

	public Node this[Point position]
	{
		get { return grid[position.X, position.Y]; }
	    set { grid[position.X, position.Y] = value; }
	}
}
