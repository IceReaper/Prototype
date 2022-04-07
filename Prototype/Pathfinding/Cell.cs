namespace Prototype.Pathfinding;

public class Cell
{
	public readonly int X;
	public readonly int Y;
	public bool IsBlocked;

	public Cell(int x, int y)
	{
		this.X = x;
		this.Y = y;
	}
}
