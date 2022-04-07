namespace Prototype.Pathfinding;

public static class PathFinder
{
	private const int MoveStraightCost = 10;
	private const int MoveDiagonalCost = 14;

	public static IEnumerable<Cell> FindPath(Grid grid, int startX, int startY, int endX, int endY)
	{
		var start = grid.GetCell(startX, startY);
		var end = grid.GetCell(endX, endY);

		if (start == null || end == null)
			return Array.Empty<Cell>();

		var openList = new List<Cell> { start };
		var closedList = new List<Cell>();
		var cellData = new Dictionary<Cell, (int GCost, int HCost, Cell? CameFrom)>();

		for (var x = 0; x < grid.Width; x++)
		for (var y = 0; y < grid.Height; y++)
		{
			var cell = grid.GetCell(x, y);

			if (cell == null)
				continue;

			cellData.Add(cell, (int.MaxValue, 0, null));
		}

		cellData[start] = (0, PathFinder.CalculateDistanceCost(start, end), null);

		while (openList.Count > 0)
		{
			var current = openList.MinBy(cell => cellData[cell].GCost + cellData[cell].GCost);

			if (current == null || current == end)
				return PathFinder.CalculatePath(end, cellData.ToDictionary(e => e.Key, e => e.Value.CameFrom));

			openList.Remove(current);
			closedList.Add(current);

			foreach (var neighbour in PathFinder.GetNeighbourList(grid, current).Where(neighbour => !closedList.Contains(neighbour)))
			{
				if (neighbour.IsBlocked)
				{
					closedList.Add(neighbour);

					continue;
				}

				var tentativeGCost = cellData[current].GCost + PathFinder.CalculateDistanceCost(current, neighbour);

				if (tentativeGCost >= cellData[neighbour].GCost)
					continue;

				cellData[neighbour] = (tentativeGCost, PathFinder.CalculateDistanceCost(neighbour, end), current);

				if (!openList.Contains(neighbour))
					openList.Add(neighbour);
			}
		}

		return Array.Empty<Cell>();
	}

	private static IEnumerable<Cell> GetNeighbourList(Grid grid, Cell cell)
	{
		return new[]
		{
			grid.GetCell(cell.X - 1, cell.Y - 1),
			grid.GetCell(cell.X - 1, cell.Y),
			grid.GetCell(cell.X - 1, cell.Y + 1),
			grid.GetCell(cell.X, cell.Y - 1),
			grid.GetCell(cell.X, cell.Y + 1),
			grid.GetCell(cell.X + 1, cell.Y - 1),
			grid.GetCell(cell.X + 1, cell.Y),
			grid.GetCell(cell.X + 1, cell.Y + 1)
		}.OfType<Cell>();
	}

	private static IEnumerable<Cell> CalculatePath(Cell end, IReadOnlyDictionary<Cell, Cell?> cameFrom)
	{
		var path = new List<Cell>();
		var current = end;

		while (current != null)
		{
			path.Add(current);
			current = cameFrom[current];
		}

		path.Reverse();

		return path;
	}

	private static int CalculateDistanceCost(Cell a, Cell b)
	{
		var xDistance = Math.Abs(a.X - b.X);
		var yDistance = Math.Abs(a.Y - b.Y);

		return PathFinder.MoveDiagonalCost * Math.Min(xDistance, yDistance) + PathFinder.MoveStraightCost * Math.Abs(xDistance - yDistance);
	}
}
