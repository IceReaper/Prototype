namespace Prototype.Scripts.EntityComponents;

using Stride.Core.Mathematics;
using Stride.Engine;
using Systems.Navigation;

public class GridComponent : StartupScript
{
	private const int MaxPathRange = 32;
	
	private Grid? grid;

	public override void Start()
	{
		this.grid = new(256, 256);

		for (var x = 0; x < 5; x++)
		for (var y = 0; y < 7; y++)
			this.grid.GetCell(115 + x, 205 + y)?.SetWall(true);
	}

	public (int X, int Y) GetCell(Vector3 position)
	{
		return this.GetPosition(position);
	}

	public bool CanTransitionToCell(Vector3 target, Entity entity)
	{
		if (this.grid == null)
			return false;

		var (startX, startY) = this.GetPosition(entity.Transform.Position);
		var (endX, endY) = this.GetPosition(target);

		return this.grid.CanTransition(entity, startX, startY, endX, endY);
	}

	public void BlockCell(Vector3 target, Entity entity)
	{
		var (x, y) = this.GetPosition(target);
		this.grid?.GetCell(x, y)?.Block(entity);
	}

	public void UnblockCell(Vector3 target, Entity entity)
	{
		var (x, y) = this.GetPosition(target);
		this.grid?.GetCell(x, y)?.Unblock(entity);
	}

	public void ReserveCell(Vector3 target, Entity entity)
	{
		var (x, y) = this.GetPosition(target);
		this.grid?.GetCell(x, y)?.Reserve(entity);
	}

	public void UnreserveCell(Vector3 target, Entity entity)
	{
		var (x, y) = this.GetPosition(target);
		this.grid?.GetCell(x, y)?.Unreserve(entity);
	}

	public IEnumerable<Vector3> FindPath(Vector3 start, Vector3 end)
	{
		if (this.grid == null)
			return Array.Empty<Vector3>();

		var (startX, startY) = this.GetPosition(start);
		var (endX, endY) = this.GetPosition(end);

		// TODO this does not work correctly yet, as if the calculated max cell is unpathable, the entities will not get a path.
		// TODO we might instead want to make paths calculate over several ticks, with a maximum number of checks per tick.
		//endX = Math.Clamp(endX, startX - GridComponent.MaxPathRange, startX + GridComponent.MaxPathRange);
		//endY = Math.Clamp(endY, startY - GridComponent.MaxPathRange, startY + GridComponent.MaxPathRange);

		return this.grid.FindPath(startX, startY, endX, endY).Select(cell => new Vector3(cell.X + .5f, 0, cell.Y + .5f) + this.Entity.Transform.Position);
	}

	private (int X, int Y) GetPosition(Vector3 position)
	{
		return ((int)(position.X - this.Entity.Transform.Position.X), (int)(position.Z - this.Entity.Transform.Position.Z));
	}
}
