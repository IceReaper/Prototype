namespace Prototype.Systems.Maps;

using FileFormats;
using Navigation;

public static class GridBuilder
{
	private static readonly List<(bool CanWalkInside, bool CanWalkOnTop, float WalkHeight)> WalkInfo = new();

	static GridBuilder()
	{
		GridBuilder.WalkInfo.Add((true, true, 1));

		for (var direction = 0; direction < 4; direction++)
		{
			GridBuilder.WalkInfo.Add((false, true, .25f));
			GridBuilder.WalkInfo.Add((false, true, .75f));
		}

		for (var direction = 0; direction < 4; direction++)
		{
			GridBuilder.WalkInfo.Add((false, true, .0625f));
			GridBuilder.WalkInfo.Add((false, true, .1875f));
			GridBuilder.WalkInfo.Add((false, true, .3125f));
			GridBuilder.WalkInfo.Add((false, true, .4375f));
			GridBuilder.WalkInfo.Add((false, true, .5625f));
			GridBuilder.WalkInfo.Add((false, true, .6875f));
			GridBuilder.WalkInfo.Add((false, true, .8125f));
			GridBuilder.WalkInfo.Add((false, true, .9375f));
		}

		for (var direction = 0; direction < 4; direction++)
			GridBuilder.WalkInfo.Add((false, true, .5f));

		for (var direction = 0; direction < 4; direction++)
			GridBuilder.WalkInfo.Add((false, false, 1));

		for (var direction = 0; direction < 4; direction++)
			GridBuilder.WalkInfo.Add((false, true, .5f));

		for (var i = 0; i < 9; i++)
			GridBuilder.WalkInfo.Add((false, false, 1));

		for (var direction = 0; direction < 4; direction++)
			GridBuilder.WalkInfo.Add((false, true, .5f));
	}

	public static Grid BuildGrid(Map map)
	{
		var grid = new Grid(map.Cells.GetLength(0), map.Cells.GetLength(1), map.Cells.GetLength(2));

		for (var x = 0; x < grid.X; x++)
		for (var y = 0; y < grid.Y; y++)
		for (var z = 0; z < grid.Z; z++)
		{
			var mapCell = map.Cells[x, y, z];

			if (mapCell.Block is { DownInner.IsHollow: false } && GridBuilder.WalkInfo[mapCell.Block.ShapeType].CanWalkInside)
			{
				// TODO this tile has a floor. Theoretically we can walk here. Find out in which directions!
			}
			else if (y > 0)
			{
				var mapCellBelow = map.Cells[x, y - 1, z];

				if (mapCellBelow.Block is { Up.IsHollow: false } && GridBuilder.WalkInfo[mapCellBelow.Block.ShapeType].CanWalkOnTop)
				{
					// TODO we can also walk over this one :)
				}
			}

			// TODO here comes the HELL OF IFS! Basically all cell checks which appends cells to other cells neighbours an use proper costs.
		}

		return grid;
	}
}
