namespace Prototype.Systems.Navigation;

using Stride.Engine;

public class Cell
{
	public readonly int X;
	public readonly int Y;

	private readonly Grid grid;
	private readonly Dictionary<Cell, float> neighbours = new();

	private bool isWall;
	private readonly List<Entity> blockedBy = new();
	private readonly List<Entity> reservedBy = new();

	public IDictionary<Cell, float> Neighbours => this.neighbours;
	public bool IsReserved => this.reservedBy.Any();

	public Cell(Grid grid, int x, int y)
	{
		this.grid = grid;
		this.X = x;
		this.Y = y;
	}

	public void SetWall(bool isWall)
	{
		if (this.isWall == isWall)
			return;

		this.isWall = isWall;
		this.UpdateNeighbours();
	}

	public void Block(Entity entity)
	{
		if (this.blockedBy.Contains(entity))
			return;

		this.blockedBy.Add(entity);
		this.UpdateNeighbours();
	}

	public void Unblock(Entity entity)
	{
		if (!this.blockedBy.Contains(entity))
			return;

		this.blockedBy.Remove(entity);
		this.UpdateNeighbours();
	}

	public void Reserve(Entity entity)
	{
		if (this.reservedBy.Contains(entity))
			return;

		this.reservedBy.Add(entity);
	}

	public void Unreserve(Entity entity)
	{
		if (!this.reservedBy.Contains(entity))
			return;

		this.reservedBy.Remove(entity);
	}

	public bool CanTransition(Entity entity, Cell target)
	{
		return this.neighbours.ContainsKey(target) && target.reservedBy.All(reserver => reserver == entity);
	}

	public void UpdateNeighbours()
	{
		for (var y = -1; y <= 1; y++)
		for (var x = -1; x <= 1; x++)
		{
			if (x == 0 && y == 0)
				continue;

			var neighbour = this.grid.GetCell(this.X + x, this.Y + y);

			if (neighbour == null)
				continue;

			if (this.CanPath(neighbour))
			{
				if (this.neighbours.ContainsKey(neighbour))
					continue;

				var cost = Grid.CalculateCost(this, neighbour);

				this.neighbours.Add(neighbour, cost);
				neighbour.neighbours.Add(this, cost);
			}
			else if (this.neighbours.ContainsKey(neighbour))
			{
				this.neighbours.Remove(neighbour);
				neighbour.neighbours.Remove(this);
			}
		}
	}

	private bool CanPath(Cell target)
	{
		var xDistance = Math.Abs(this.X - target.X);
		var yDistance = Math.Abs(this.Y - target.Y);

		if (this.IsBlocked() || target.IsBlocked())
			return false;

		if (xDistance != 1 || yDistance != 1)
			return true;

		return new[] { this.grid.GetCell(this.X, target.Y), this.grid.GetCell(target.X, this.Y) }.Count(cell => cell != null && !cell.IsBlocked()) == 2;
	}

	private bool IsBlocked()
	{
		return this.isWall || this.blockedBy.Any();
	}
}
