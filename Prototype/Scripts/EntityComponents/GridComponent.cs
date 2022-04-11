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

	public override void Start()
	{
		this.grid = new(256, 256);

		this.grid = new(20, 20);
		this.Entity.Transform.Position = new(115, 2, 205);

		/*for (var x = 5; x <= 5; x++)
		for (var y = 0; y <= 7; y++)
			this.grid.GetCell(115 + x, 205 + y)?.SetWall(true);*/

		var modelFree = new Model
		{
			new Mesh { Draw = GeometricPrimitive.Cube.New(this.GraphicsDevice, .5f).ToMeshDraw() },
			Material.New(
				this.GraphicsDevice,
				new()
				{
					Attributes =
					{
						Diffuse = new MaterialDiffuseMapFeature(new ComputeColor(Color.White)),
						DiffuseModel = new MaterialDiffuseLambertModelFeature()
					}
				}
			)
		};

		var modelBlocked = new Model
		{
			new Mesh { Draw = GeometricPrimitive.Cube.New(this.GraphicsDevice, .5f).ToMeshDraw() },
			Material.New(
				this.GraphicsDevice,
				new()
				{
					Attributes =
					{
						Diffuse = new MaterialDiffuseMapFeature(new ComputeColor(Color.Red)),
						DiffuseModel = new MaterialDiffuseLambertModelFeature()
					}
				}
			)
		};

		for (var x = 0; x < 20; x++)
		for (var y = 0; y < 20; y++)
		{
			var model = modelFree;

			if (x == 5 && y is >= 0 and <= 7)
			{
				model = modelBlocked;
				this.grid.GetCell(x, y)?.SetWall(true);
			}

			var debugcube = new Entity { new ModelComponent(model) };
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

		return this.grid.CanTransition(startX, startY, endX, endY);
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

		return this.grid.FindPath(startX, startY, endX, endY).Select(cell => new Vector3(cell.X + .5f, 0, cell.Y + .5f) + this.Entity.Transform.Position);
	}

	private (int X, int Y) GetPosition(Vector3 position)
	{
		return ((int)(position.X - this.Entity.Transform.Position.X), (int)(position.Z - this.Entity.Transform.Position.Z));
	}
}
