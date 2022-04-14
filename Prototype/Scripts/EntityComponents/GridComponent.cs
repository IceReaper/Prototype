namespace Prototype.Scripts.EntityComponents;

using Stride.Core.Mathematics;
using Stride.Engine;
using Systems.Navigation;

public class GridComponent : StartupScript
{
	public Grid? Grid;

	public Cell? GetCellContaining(Vector3 position)
	{
		var (x, y, z) = ((int)position.X, (int)position.Y, (int)position.Z);

		if (this.Grid != null && x >= 0 && y >= 0 && z >= 0 && x < this.Grid.X && y < this.Grid.Y && z < this.Grid.Z)
			return this.Grid.Cells[x, y, z];

		return null;
	}

	public IEnumerable<Cell> FindPath(Vector3 from, Vector3 to)
	{
		if (this.Grid == null)
			return Array.Empty<Cell>();

		if (from.X >= 0 && from.Y >= 0 && from.Z >= 0 && from.X < this.Grid.X && from.Y < this.Grid.Y && from.Z < this.Grid.Z)
			return Array.Empty<Cell>();

		if (to.X >= 0 && to.Y >= 0 && to.Z >= 0 && to.X < this.Grid.X && to.Y < this.Grid.Y && to.Z < this.Grid.Z)
			return Array.Empty<Cell>();

		return Pathfinder.FindPath(this.Grid, (int)from.X, (int)from.Y, (int)from.Z, (int)to.X, (int)to.Y, (int)to.Z);
	}
}
