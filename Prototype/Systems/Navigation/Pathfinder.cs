namespace Prototype.Systems.Navigation;

public static class Pathfinder
{
	private class CellInfo
	{
		public Cell? Previous;
		public Cell? Next;
		public float CostFromStart;
		public float CostToEnd;
		public float CostFromStartEstimated;
		public float CostToEndEstimated;
	}

	public const float MoveStraightCost = 1;
	public const float MoveDiagonalCost = 1.4f;
	public const float MoveUpCost = 1.4f;
	public const float ReservedCost = 3;

	public static IEnumerable<Cell> FindPath(Grid grid, int startX, int startY, int startZ, int endX, int endY, int endZ)
	{
		var start = grid.Cells[startX, startY, startZ];
		var end = grid.Cells[endX, endY, endZ];

		if (start == end)
			return Array.Empty<Cell>();

		var visitedCells = new Dictionary<Cell, CellInfo>(grid.X * grid.Y * grid.Z)
		{
			{ start, new() { CostToEndEstimated = Pathfinder.CalculateCost(start, end) } },
			{ end, new() { CostFromStartEstimated = Pathfinder.CalculateCost(end, start) } }
		};

		var openListStart = new List<Cell> { start };
		var openListEnd = new List<Cell> { end };

		Cell? matchCell = null;
		var matchCost = float.MaxValue;

		while (true)
		{
			var bestFromStart = openListStart.MinBy(cell => visitedCells[cell].CostToEndEstimated);
			var bestFromEnd = openListEnd.MinBy(cell => visitedCells[cell].CostFromStartEstimated);

			if (bestFromStart == null || bestFromEnd == null)
				break;

			openListStart.Remove(bestFromStart);
			openListEnd.Remove(bestFromEnd);

			foreach (var neighbour in bestFromStart.Neighbours.Keys.Where(cell => cell != start))
			{
				CellInfo? info = null;

				var costFromStart = visitedCells[bestFromStart].CostFromStart
					+ bestFromStart.Neighbours[neighbour]
					+ (neighbour.Reservers.Any() ? Pathfinder.ReservedCost : 0);

				var costToEndEstimated = Pathfinder.CalculateCost(neighbour, end);

				if (matchCell != null && costFromStart + costToEndEstimated > matchCost)
					continue;

				if (!visitedCells.ContainsKey(neighbour))
				{
					info = new();
					openListStart.Add(neighbour);
					visitedCells.Add(neighbour, info);
				}
				else if ((visitedCells[neighbour].Next != null || neighbour == end) && visitedCells[neighbour].Previous == null)
					info = visitedCells[neighbour];

				if (info == null)
					continue;

				info.Previous = bestFromStart;
				info.CostFromStart = costFromStart;
				info.CostToEndEstimated = costToEndEstimated;

				if ((info.Next == null && neighbour != end) || info.CostFromStart + info.CostToEnd >= matchCost)
					continue;

				matchCell = neighbour;
				matchCost = info.CostFromStart + info.CostToEnd;

				openListStart.RemoveAll(cell => visitedCells[cell].CostFromStart + visitedCells[cell].CostToEndEstimated >= matchCost);
				openListEnd.RemoveAll(cell => visitedCells[cell].CostFromStartEstimated + visitedCells[cell].CostToEnd >= matchCost);
			}

			foreach (var neighbour in bestFromEnd.Neighbours.Keys.Where(cell => cell != end))
			{
				CellInfo? info = null;

				var costFromStartEstimated = Pathfinder.CalculateCost(start, neighbour);

				var costToEnd = visitedCells[bestFromEnd].CostToEnd
					+ neighbour.Neighbours[bestFromEnd]
					+ (bestFromEnd.Reservers.Any() ? Pathfinder.ReservedCost : 0);

				if (matchCell != null && costFromStartEstimated + costToEnd > matchCost)
					continue;

				if (!visitedCells.ContainsKey(neighbour))
				{
					info = new();
					openListEnd.Add(neighbour);
					visitedCells.Add(neighbour, info);
				}
				else if ((visitedCells[neighbour].Previous != null || neighbour == start) && visitedCells[neighbour].Next == null)
					info = visitedCells[neighbour];

				if (info == null)
					continue;

				info.Next = bestFromEnd;
				info.CostFromStartEstimated = costFromStartEstimated;
				info.CostToEnd = costToEnd;

				if ((info.Previous == null && neighbour != start) || info.CostFromStart + info.CostToEnd >= matchCost)
					continue;

				matchCell = neighbour;
				matchCost = info.CostFromStart + info.CostToEnd;

				openListStart.RemoveAll(cell => visitedCells[cell].CostFromStart + visitedCells[cell].CostToEndEstimated >= matchCost);
				openListEnd.RemoveAll(cell => visitedCells[cell].CostFromStartEstimated + visitedCells[cell].CostToEnd >= matchCost);
			}
		}

		if (matchCell == null)
			return Array.Empty<Cell>();

		var path = new List<Cell> { matchCell };

		for (var previous = visitedCells[matchCell].Previous; previous != null; previous = visitedCells[previous].Previous)
			path.Insert(0, previous);

		for (var next = visitedCells[matchCell].Next; next != null; next = visitedCells[next].Next)
			path.Add(next);

		return path.Skip(1);
	}

	private static float CalculateCost(Cell from, Cell to)
	{
		var xDistance = Math.Abs(from.X - to.X);
		var yDistance = Math.Abs(from.Y - to.Y);
		var zDistance = Math.Abs(from.Z - to.Z);

		return Pathfinder.MoveDiagonalCost * Math.Min(xDistance, yDistance)
			+ Pathfinder.MoveStraightCost * Math.Abs(xDistance - yDistance)
			+ Pathfinder.MoveUpCost * zDistance;
	}
}
