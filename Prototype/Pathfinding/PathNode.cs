namespace Prototype.Pathfinding;

public class PathNode
{
	private readonly Grid grid;
	public readonly int X;
	public readonly int Y;

	// Costs for horizontal and diagonal movement
	public int GCost;
	public int HCost;
	public int FCost;

	// Bool for blocking the cell
	public bool IsWalkable;

	public PathNode? CameFromNode; //previous Node

	public PathNode(Grid grid, int x, int y)
	{
		this.grid = grid;
		this.X = x;
		this.Y = y;
		this.IsWalkable = true;
	}

	public void SetIsWalkable(bool isWalkable)
	{
		this.IsWalkable = isWalkable;
		this.grid.TriggerGridObjectChanged(this.X, this.Y, this);
	}

	public void CalculateFCost()
	{
		this.FCost = this.GCost + this.HCost;
	}
}