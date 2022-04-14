namespace Prototype.Systems.Navigation;

using Stride.Engine;

public class Cell
{
	public readonly int X;
	public readonly int Y;
	public readonly int Z;

	public readonly Dictionary<Cell, float> Neighbours = new();
	public readonly List<Entity> Reservers = new();

	public Cell(int x, int y, int z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}
}
