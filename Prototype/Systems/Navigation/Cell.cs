namespace Prototype.Systems.Navigation;

using Stride.Engine;

public sealed class Cell
{
	public readonly int X;
	public float Y;
	public readonly int Z;

	public readonly Dictionary<Cell, float> Neighbours = new();
	public readonly List<Entity> Occupiers = new();

	public Cell(int x, int y, int z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}
}
