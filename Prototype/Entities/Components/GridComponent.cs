namespace Prototype.Entities.Components;

using Pathfinding;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;

public class GridComponent : StartupScript
{
	private Grid? grid;
	private Model[,] models = new Model[0, 0];
	private Material? materialFree;
	private Material? materialBlocked;

	public override void Start()
	{
		this.grid = new(20, 20);
		this.models = new Model[this.grid.Width, this.grid.Height];

		this.materialFree = Material.New(
			this.GraphicsDevice,
			new()
			{
				Attributes =
				{
					Diffuse = new MaterialDiffuseMapFeature(new ComputeColor(Color.White)), DiffuseModel = new MaterialDiffuseLambertModelFeature()
				}
			}
		);

		this.materialBlocked = Material.New(
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
						new Mesh { Draw = GeometricPrimitive.Cube.New(this.GraphicsDevice, .5f, 8).ToMeshDraw() }, this.materialFree
					}
				)
			};

			debugcube.Transform.Position = new(x + .5f, 0, y + .5f);
			this.Entity.AddChild(debugcube);
		}
	}

	public IEnumerable<Vector3> FindPath(Vector3 start, Vector3 end)
	{
		if (this.grid == null)
			return Array.Empty<Vector3>();

		var startX = (int)(start.X - this.Entity.Transform.Position.X);
		var startY = (int)(start.Z - this.Entity.Transform.Position.Z);
		var endX = (int)(end.X - this.Entity.Transform.Position.X);
		var endY = (int)(end.Z - this.Entity.Transform.Position.Z);

		return PathFinder.FindPath(this.grid, startX, startY, endX, endY)
			.Select(cell => new Vector3(cell.X + .5f, 0, cell.Y + .5f) + this.Entity.Transform.Position);
	}

	public void ToggleWalkable(Vector3 origin)
	{
		if (this.grid == null)
			return;

		var x = (int)(origin.X - this.Entity.Transform.Position.X);
		var y = (int)(origin.Z - this.Entity.Transform.Position.Z);

		var cell = this.grid.GetCell(x, y);

		if (cell == null)
			return;

		cell.IsBlocked = !cell.IsBlocked;
		this.models[x, y].Materials[0] = cell.IsBlocked ? this.materialBlocked : this.materialFree;
	}
}
