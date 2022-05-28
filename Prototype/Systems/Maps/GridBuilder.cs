namespace Prototype.Systems.Maps;

using FileFormats;
using Navigation;
using Cell = FileFormats.Cell;

public static class GridBuilder
{
	private static readonly List<(bool CanWalkOnTop, float WalkHeight)> WalkInfo = new();

	static GridBuilder()
	{
		GridBuilder.WalkInfo.Add((true, 1));

		for (var direction = 0; direction < 4; direction++)
		{
			GridBuilder.WalkInfo.Add((true, .25f));
			GridBuilder.WalkInfo.Add((true, .75f));
		}

		for (var direction = 0; direction < 4; direction++)
		{
			GridBuilder.WalkInfo.Add((true, .0625f));
			GridBuilder.WalkInfo.Add((true, .1875f));
			GridBuilder.WalkInfo.Add((true, .3125f));
			GridBuilder.WalkInfo.Add((true, .4375f));
			GridBuilder.WalkInfo.Add((true, .5625f));
			GridBuilder.WalkInfo.Add((true, .6875f));
			GridBuilder.WalkInfo.Add((true, .8125f));
			GridBuilder.WalkInfo.Add((true, .9375f));
		}

		for (var direction = 0; direction < 4; direction++)
			GridBuilder.WalkInfo.Add((true, .5f));

		for (var direction = 0; direction < 4; direction++)
			GridBuilder.WalkInfo.Add((false, 1));

		for (var direction = 0; direction < 4; direction++)
			GridBuilder.WalkInfo.Add((true, .5f));

		for (var i = 0; i < 9; i++)
			GridBuilder.WalkInfo.Add((false, 1));

		for (var direction = 0; direction < 4; direction++)
			GridBuilder.WalkInfo.Add((true, .5f));
	}

	public static Grid BuildGrid(Map map)
	{
		var grid = new Grid(map.Cells.GetLength(0), map.Cells.GetLength(1), map.Cells.GetLength(2));

		for (var x = 0; x < grid.X; x++)
		for (var y = 0; y < grid.Y; y++)
		for (var z = 0; z < grid.Z; z++)
		{
			var currentCell = map.Cells[x, y, z];
			var currentCellBelow = y == 0 ? null : map.Cells[x, y - 1, z];

			var cellIsBlocked = GridBuilder.CellIsBlocked(currentCell);
			var cellHasFloor = GridBuilder.CellHasFloor(currentCell, currentCellBelow);

			if (cellIsBlocked || !cellHasFloor)
				continue;

			for (var nextX = x - 1; nextX <= x + 1; nextX++)
			for (var nextZ = z - 1; nextZ <= z + 1; nextZ++)
			{
				if (nextX < 0 || nextX >= grid.X || nextZ < 0 || nextZ >= grid.Z || (nextX == x && nextZ == z))
					continue;

				var nextCell = map.Cells[nextX, y, nextZ];
				var nextCellBelow = y == 0 ? null : map.Cells[x, y - 1, z];

				var nextCellIsBlocked = GridBuilder.CellIsBlocked(nextCell);
				var nextCellHasFloor = GridBuilder.CellHasFloor(nextCell, nextCellBelow);

				if (nextCellIsBlocked || !nextCellHasFloor)
					continue;

				// TODO check walls!

				var a = grid.Cells[x, y, z];
				var b = grid.Cells[nextX, y, nextZ];

				if (a.Neighbours.ContainsKey(b))
					continue;

				a.Neighbours.Add(b, Pathfinder.CalculateCost(a, b));
				b.Neighbours.Add(a, Pathfinder.CalculateCost(b, a));
			}
		}

		return grid;
	}

	private static bool CellIsBlocked(Cell mapCell)
	{
		if (mapCell.Block == null)
			return false;

		return mapCell.Block.ShapeType switch
		{
			>= 1 and <= 44 when mapCell.Block.Up is { IsHollow: false } || mapCell.Block.UpInner is { IsHollow: false } => true,
			45 or 47 when mapCell.Block.Left is { IsHollow: false } || mapCell.Block.LeftInner is { IsHollow: false } => true,
			46 or 48 when mapCell.Block.Right is { IsHollow: false } || mapCell.Block.RightInner is { IsHollow: false } => true,
			49 or 51 when mapCell.Block.Left is { IsHollow: false } || mapCell.Block.LeftInner is { IsHollow: false } => true,
			50 or 52 when mapCell.Block.Right is { IsHollow: false } || mapCell.Block.RightInner is { IsHollow: false } => true,
			61 when mapCell.Block.Right is { IsHollow: false }
				|| mapCell.Block.RightInner is { IsHollow: false }
				|| mapCell.Block.Left is { IsHollow: false }
				|| mapCell.Block.LeftInner is { IsHollow: false }
				|| mapCell.Block.Forward is { IsHollow: false }
				|| mapCell.Block.ForwardInner is { IsHollow: false }
				|| mapCell.Block.Backward is { IsHollow: false }
				|| mapCell.Block.BackwardInner is { IsHollow: false } => true,
			62 or 64 when mapCell.Block.Left is { IsHollow: false } || mapCell.Block.LeftInner is { IsHollow: false } => true,
			63 or 65 when mapCell.Block.Right is { IsHollow: false } || mapCell.Block.RightInner is { IsHollow: false } => true,
			_ => false
		};
	}

	private static bool CellHasFloor(Cell mapCell, Cell? mapCellBelow)
	{
		if (mapCell.Block?.ShapeType is 0 or >= 1 and <= 44 or >= 49 and <= 52 && mapCell.Block.DownInner is { IsHollow: false })
			return true;

		return mapCellBelow?.Block?.ShapeType is 0 or >= 1 and <= 40 && mapCellBelow.Block.Up is { IsHollow: false };
	}
}
