namespace Prototype.Systems.Maps;

using FileFormats;
using Navigation;

public static class GridBuilder
{
	public static Grid BuildGrid(Map map)
	{
		var grid = new Grid(map.Cells.GetLength(0), map.Cells.GetLength(1), map.Cells.GetLength(2));

		for (var x = 0; x < grid.X; x++)
		for (var y = 0; y < grid.Y; y++)
		for (var z = 0; z < grid.Z; z++)
		{
			var cell = grid.Cells[x, y, z];

			// TODO here comes the HELL OF IFS! Basically all cell checks which appends cells to other cells neighbours an use proper costs.
		}

		return grid;
	}
}
