namespace Prototype.Systems.Navigation;

public class Grid
{
	public readonly int Width;
	public readonly int Height;
	private readonly Cell[,] cells;

	public Grid(int width, int height)
	{
		this.Width = width;
		this.Height = height;

		this.cells = new Cell[width, height];

		for (var x = 0; x < this.cells.GetLength(0); x++)
		for (var y = 0; y < this.cells.GetLength(1); y++)
			this.cells[x, y] = new(x, y);
	}

	public Cell? GetCell(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < this.Width && y < this.Height)
			return this.cells[x, y];

		return null;
	}
}
