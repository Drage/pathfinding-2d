public class Node
{
	public Point Position;
	public bool Passable;
	public int Cost;
	
	public int X { get { return Position.X; } }
    public int Y { get { return Position.Y; } }

    public Grid Grid { get; private set; }
	
	public Node(Grid grid, int x, int y)
	{
		this.Grid = grid;
		this.Position = new Point(x, y);
		this.Passable = true;
		this.Cost = 1;
	}
}
