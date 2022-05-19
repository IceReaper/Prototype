namespace Prototype.Systems.Navigation;

public sealed class Grid
{
	public readonly int X;
	public readonly int Y;
	public readonly int Z;

	public readonly Cell[,,] Cells;

	public Grid(int x, int y, int z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;

		this.Cells = new Cell[x, y, z];

		for (x = 0; x < this.X; x++)
		for (y = 0; y < this.Y; y++)
		for (z = 0; z < this.Z; z++)
			this.Cells[x, y, z] = new(x, y, z);
	}
}
