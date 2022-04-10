namespace Prototype.Scripts.EntityComponents;

using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;
using Systems.Navigation;

public class GridComponent : StartupScript
{
	private Grid? grid;
	private Model[,] models = new Model[0, 0];

	public override void Start()
	{
		this.grid = new(20, 20);
		this.models = new Model[this.grid.Width, this.grid.Height];

		for (var y = 0; y <= 7; y++)
		for (var x = 5; x <= 5; x++)
		{
			var cell = this.grid.GetCell(x, y);

			if (cell != null)
				cell.IsWall = true;
		}

		var materialFree = Material.New(
			this.GraphicsDevice,
			new()
			{
				Attributes =
				{
					Diffuse = new MaterialDiffuseMapFeature(new ComputeColor(Color.White)), DiffuseModel = new MaterialDiffuseLambertModelFeature()
				}
			}
		);

		var materialBlocked = Material.New(
			this.GraphicsDevice,
			new()
			{
				Attributes =
				{
					Diffuse = new MaterialDiffuseMapFeature(new ComputeColor(Color.Red)), DiffuseModel = new MaterialDiffuseLambertModelFeature()
				}
			}
		);

		for (var x = 0; x < this.grid.Width; x++)
		for (var y = 0; y < this.grid.Height; y++)
		{
			var debugcube = new Entity
			{
				new ModelComponent(
					this.models[x, y] = new()
					{
						new Mesh { Draw = GeometricPrimitive.Cube.New(this.GraphicsDevice, .5f, 8).ToMeshDraw() },
						this.grid.GetCell(x, y)?.IsWall ?? false ? materialBlocked : materialFree
					}
				)
			};

			debugcube.Transform.Position = new(x + .5f, 0, y + .5f);
			this.Entity.AddChild(debugcube);
		}
	}

	public bool CanTransitionToCell(Vector3 start, Vector3 end)
	{
		if (this.grid == null)
			return false;

		var (startX, startY) = this.GetPosition(start);
		var (endX, endY) = this.GetPosition(end);

		return PathFinder.CanTransitionToCell(this.grid, startX, startY, endX, endY);
	}

	public void BlockCell(Vector3 target, Entity entity)
	{
		var (x, y) = this.GetPosition(target);
		this.grid?.GetCell(x, y)?.BlockedBy.Add(entity);
	}

	public void UnblockCell(Vector3 target, Entity entity)
	{
		var (x, y) = this.GetPosition(target);
		this.grid?.GetCell(x, y)?.BlockedBy.Remove(entity);
	}

	public void ReserveCell(Vector3 target, Entity entity)
	{
		var (x, y) = this.GetPosition(target);
		this.grid?.GetCell(x, y)?.ReservedBy.Add(entity);
	}

	public void UnreserveCell(Vector3 target, Entity entity)
	{
		var (x, y) = this.GetPosition(target);
		this.grid?.GetCell(x, y)?.ReservedBy.Remove(entity);
	}

	public IEnumerable<Vector3> FindPath(Vector3 start, Vector3 end)
	{
		if (this.grid == null)
			return Array.Empty<Vector3>();

		var (startX, startY) = this.GetPosition(start);
		var (endX, endY) = this.GetPosition(end);

		return PathFinder.FindPath(this.grid, startX, startY, endX, endY)
			.Select(cell => new Vector3(cell.X + .5f, 0, cell.Y + .5f) + this.Entity.Transform.Position);
	}

	private (int X, int Y) GetPosition(Vector3 position)
	{
		return ((int)(position.X - this.Entity.Transform.Position.X), (int)(position.Z - this.Entity.Transform.Position.Z));
	}
}
