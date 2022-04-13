namespace Prototype.Systems.Navigation;

public class Grid
{
	private const float MoveStraightCost = 1;
	private const float MoveDiagonalCost = 1.4f;
	private const float ReservedCost = 3;

	private class CellInfo
	{
		public Cell? Previous;
		public Cell? Next;
		public float CostFromStart;
		public float CostToEnd;
		public float CostFromStartEstimated;
		public float CostToEndEstimated;
	}

	private readonly int width;
	private readonly int height;

	private readonly Cell[,] cells;

	public Grid(int width, int height)
	{
		this.width = width;
		this.height = height;

		this.cells = new Cell[width, height];

		for (var x = 0; x < this.width; x++)
		for (var y = 0; y < this.height; y++)
			this.cells[x, y] = new(this, x, y);

		for (var x = 0; x < this.width; x++)
		for (var y = 0; y < this.height; y++)
			this.cells[x, y].UpdateNeighbours();
	}

	public Cell? GetCell(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < this.width && y < this.height)
			return this.cells[x, y];

		return null;
	}

	public bool CanTransition(int startX, int startY, int endX, int endY)
	{
		var start = this.GetCell(startX, startY);
		var end = this.GetCell(endX, endY);

		if (start == null || end == null || start == end)
			return false;

		return start.CanTransition(end);
	}

	public IEnumerable<Cell> FindPath(int startX, int startY, int endX, int endY)
	{
		var start = this.GetCell(startX, startY);
		var end = this.GetCell(endX, endY);

		if (start == null || end == null || start == end)
			return Array.Empty<Cell>();

		var visitedCells = new Dictionary<Cell, CellInfo>(this.width * this.height)
		{
			{ start, new() { CostToEndEstimated = Grid.CalculateCost(start, end) } },
			{ end, new() { CostFromStartEstimated = Grid.CalculateCost(end, start) } }
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
					+ (neighbour.IsReserved ? Grid.ReservedCost : 0);

				var costToEndEstimated = Grid.CalculateCost(neighbour, end);

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
				var costFromStartEstimated = Grid.CalculateCost(start, neighbour);
				var costToEnd = visitedCells[bestFromEnd].CostToEnd + neighbour.Neighbours[bestFromEnd] + (bestFromEnd.IsReserved ? Grid.ReservedCost : 0);

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

	public static float CalculateCost(Cell from, Cell to)
	{
		var xDistance = Math.Abs(from.X - to.X);
		var yDistance = Math.Abs(from.Y - to.Y);

		return Grid.MoveDiagonalCost * Math.Min(xDistance, yDistance) + Grid.MoveStraightCost * Math.Abs(xDistance - yDistance);
	}
}
