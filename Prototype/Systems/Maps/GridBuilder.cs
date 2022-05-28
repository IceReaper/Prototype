namespace Prototype.Systems.Maps;

using FileFormats;
using Navigation;
using Cell = FileFormats.Cell;

public static class GridBuilder
{
	private static readonly List<float> WalkHeights = new();

	static GridBuilder()
	{
		for (var direction = 0; direction < 4; direction++)
		{
			GridBuilder.WalkHeights.Add(.25f);
			GridBuilder.WalkHeights.Add(.75f);
		}

		for (var direction = 0; direction < 4; direction++)
		{
			GridBuilder.WalkHeights.Add(.0625f);
			GridBuilder.WalkHeights.Add(.1875f);
			GridBuilder.WalkHeights.Add(.3125f);
			GridBuilder.WalkHeights.Add(.4375f);
			GridBuilder.WalkHeights.Add(.5625f);
			GridBuilder.WalkHeights.Add(.6875f);
			GridBuilder.WalkHeights.Add(.8125f);
			GridBuilder.WalkHeights.Add(.9375f);
		}
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

			if (currentCellBelow?.Block?.ShapeType is >= 1 and <= 40)
				grid.Cells[x, y, z].Y -= 1 - GridBuilder.WalkHeights[currentCellBelow.Block.ShapeType - 1];

			for (var nextX = x - 1; nextX <= x + 1; nextX++)
			for (var nextZ = z - 1; nextZ <= z + 1; nextZ++)
			{
				if (nextX < 0 || nextX >= grid.X || nextZ < 0 || nextZ >= grid.Z || (nextX == x && nextZ == z))
					continue;

				var nextY = y;

				if ((currentCellBelow?.Block?.ShapeType is 1 or 9 && nextZ > z)
				    || (currentCellBelow?.Block?.ShapeType is 3 or 17 && nextZ < z)
				    || (currentCellBelow?.Block?.ShapeType is 5 or 25 && nextX > x)
				    || (currentCellBelow?.Block?.ShapeType is 7 or 33 && nextX < x))
				{
					nextY -= 1;

					if (nextY == 0)
						continue;
				}

				var nextCell = map.Cells[nextX, nextY, nextZ];
				var nextCellBelow = nextY == 0 ? null : map.Cells[nextX, nextY - 1, nextZ];

				var nextCellIsBlocked = GridBuilder.CellIsBlocked(nextCell);
				var nextCellHasFloor = GridBuilder.CellHasFloor(nextCell, nextCellBelow);

				if (nextCellIsBlocked || !nextCellHasFloor)
					continue;

				// TODO check walls!

				var a = grid.Cells[x, y, z];
				var b = grid.Cells[nextX, nextY, nextZ];

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
