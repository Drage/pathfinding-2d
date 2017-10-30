using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Path<Node> : IEnumerable<Node>
{
	public Node LastStep { get; private set; }
    public Path<Node> PreviousSteps { get; private set; }
    public double TotalCost { get; private set; }
	
	private IEnumerator<Node> enumerator;
	
    private Path(Node lastStep, Path<Node> previousSteps, double totalCost)
    {
        LastStep = lastStep;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
    }
	
    public Path(Node start) : this(start, null, 0) {}
	
    public Path<Node> AddStep(Node step, double stepCost)
    {
        return new Path<Node>(step, this, TotalCost + stepCost);
    }
	
    public IEnumerator<Node> GetEnumerator()
    {
        for (Path<Node> p = this; p != null; p = p.PreviousSteps)
            yield return p.LastStep;
    }
	
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
	
	public Node StartTraversal()
	{
		enumerator = this.Reverse().GetEnumerator();
		enumerator.MoveNext();
		return enumerator.Current;
	}
	
	public Node NextNode()
	{
		enumerator.MoveNext();
		return enumerator.Current;
	}
	
	public Node Origin
	{
		get { return this.Last(); }
	}
	
	public Node Destination
	{
		get { return this.First(); }
	}
}
