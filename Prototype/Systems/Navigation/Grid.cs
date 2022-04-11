namespace Prototype.Systems.Navigation;

public class Grid
{
	private const float MoveStraightCost = 1;
	private const float MoveDiagonalCost = 1.4f;
	private const float ReservedCost = 5;

	private struct CellInfo
	{
		public float CostRequired;
		public float CostRemaining;
		public Cell? CameFrom;
		public bool FromStart;
	}

	private readonly int width;
	private readonly int height;

	private readonly Cell[,] cells;
	private readonly Dictionary<Cell, CellInfo> cellInfos;

	public Grid(int width, int height)
	{
		this.width = width;
		this.height = height;

		this.cells = new Cell[width, height];
		this.cellInfos = new(this.width * this.height);

		for (var x = 0; x < this.cells.GetLength(0); x++)
		for (var y = 0; y < this.cells.GetLength(1); y++)
			this.cells[x, y] = new(this, x, y);
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

		this.cellInfos.Clear();
		this.cellInfos.Add(start, new() { CostRemaining = Grid.CalculateCost(start, end), FromStart = true });
		this.cellInfos.Add(end, new() { CostRemaining = Grid.CalculateCost(end, start), FromStart = false });

		var openListStart = new List<Cell> { start };
		var openListEnd = new List<Cell> { end };
		IEnumerable<Cell>? path = null;

		while (openListStart.Count > 0 && openListEnd.Count > 0)
		{
			path ??= this.ProcessNode(openListStart, true, end);
			path ??= this.ProcessNode(openListEnd, false, start);

			if (path != null)
				return path;
		}

		return Array.Empty<Cell>();
	}

	private IEnumerable<Cell>? ProcessNode(ICollection<Cell> open, bool fromStart, Cell end)
	{
		var current = open.MinBy(cell => this.cellInfos[cell].CostRemaining);

		if (current == null)
			return null;

		open.Remove(current);
		
		// TODO when we have an connection, stop doing end pathing. And from now on, including this iteration:
		// TODO if a cell connects throw away all end-paths and open start cells with higher value than this one.
		// TODO only add start cells which can theoretically outpass the current highest value
		// TODO when we are out of open nodes, we precisely know which connection was the shortest.
		foreach (var (neighbour, cost) in current.Neighbours)
		{
			if (!this.cellInfos.ContainsKey(neighbour))
			{
				open.Add(neighbour);

				this.cellInfos.Add(
					neighbour,
					new()
					{
						CostRequired = this.cellInfos[current].CostRequired + cost + (neighbour.IsReserved ? Grid.ReservedCost : 0),
						CostRemaining = Grid.CalculateCost(neighbour, end),
						CameFrom = current,
						FromStart = fromStart
					}
				);
			}
			else if (this.cellInfos[neighbour].FromStart != fromStart)
			{
				var pathA = Grid.CalculatePath(current, this.cellInfos.ToDictionary(e => e.Key, e => e.Value.CameFrom));
				var pathB = Grid.CalculatePath(neighbour, this.cellInfos.ToDictionary(e => e.Key, e => e.Value.CameFrom));

				return fromStart ? pathA.Skip(1).Concat(pathB.Reverse()) : pathB.Skip(1).Concat(pathA.Reverse());
			}
		}

		return null;
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

	public static float CalculateCost(Cell from, Cell to)
	{
		var xDistance = Math.Abs(from.X - to.X);
		var yDistance = Math.Abs(from.Y - to.Y);

		return Grid.MoveDiagonalCost * Math.Min(xDistance, yDistance) + Grid.MoveStraightCost * Math.Abs(xDistance - yDistance);
	}
}
