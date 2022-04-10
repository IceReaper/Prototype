namespace Prototype.Systems.Navigation;

using Stride.Engine;

public class Cell
{
	public readonly int X;
	public readonly int Y;
	public bool IsWall;
	public readonly List<Entity> BlockedBy = new();
	public readonly List<Entity> ReservedBy = new();

	public Cell(int x, int y)
	{
		this.X = x;
		this.Y = y;
	}
}
