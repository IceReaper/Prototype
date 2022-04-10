namespace Prototype.Systems.Navigation;

public static class PathFinder
{
	private const int MoveStraightCost = 10;
	private const int MoveDiagonalCost = 14;

	public static bool CanTransitionToCell(Grid grid, int startX, int startY, int endX, int endY)
	{
		var start = grid.GetCell(startX, startY);
		var end = grid.GetCell(endX, endY);

		if (start == null || end == null || start == end)
			return false;

		if (end.IsWall || end.BlockedBy.Any() || end.ReservedBy.Any())
			return false;

		if (start.X != end.X && start.Y != end.Y)
		{
			if (!PathFinder.CanTransitionToCell(grid, startX, startY, startX, endY))
				return false;

			if (!PathFinder.CanTransitionToCell(grid, startX, startY, endX, startY))
				return false;
		}

		return true;
	}

	public static IEnumerable<Cell> FindPath(Grid grid, int startX, int startY, int endX, int endY)
	{
		var start = grid.GetCell(startX, startY);
		var end = grid.GetCell(endX, endY);

		if (start == null || end == null || start == end)
			return Array.Empty<Cell>();

		var openList = new List<Cell> { start };
		var cellData = new Dictionary<Cell, (int GCost, int HCost, Cell? CameFrom)> { { start, (0, PathFinder.CalculateDistanceCost(start, end), null) } };

		while (openList.Count > 0)
		{
			var current = openList.MinBy(
				cell =>
				{
					if (!cellData.ContainsKey(cell))
						cellData.Add(cell, (int.MaxValue, 0, null));

					return cellData[cell].GCost + cellData[cell].GCost;
				}
			);

			if (current == null || current == end)
				return PathFinder.CalculatePath(end, cellData.ToDictionary(e => e.Key, e => e.Value.CameFrom));

			openList.Remove(current);

			foreach (var neighbour in PathFinder.GetNeighbourList(grid, current))
			{
				if (!PathFinder.CanTransitionToCell(grid, current.X, current.Y, neighbour.X, neighbour.Y))
					continue;

				if (!cellData.ContainsKey(neighbour))
					cellData.Add(neighbour, (int.MaxValue, 0, null));

				var tentativeGCost = cellData[current].GCost + PathFinder.CalculateDistanceCost(current, neighbour);

				if (tentativeGCost >= cellData[neighbour].GCost)
					continue;

				cellData[neighbour] = (tentativeGCost, PathFinder.CalculateDistanceCost(neighbour, end), current);

				if (!openList.Contains(neighbour))
					openList.Add(neighbour);
			}
		}

		var nearest = cellData.Where(cell => cell.Value.CameFrom != null && cell.Value.HCost < cellData[start].HCost)
			.OrderBy(cell => cell.Value.HCost)
			.ToArray();

		return nearest.Any() ? PathFinder.CalculatePath(nearest.First().Key, cellData.ToDictionary(e => e.Key, e => e.Value.CameFrom)) : Array.Empty<Cell>();
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
