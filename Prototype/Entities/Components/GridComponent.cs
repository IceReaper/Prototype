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

	public override void Start()
	{
		this.grid = new(20, 20, (g, x, y) => new(g, x, y));
		this.models = new Model[this.grid.Width, this.grid.Height];

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
					this.models[x, y] = new() { new Mesh { Draw = GeometricPrimitive.Cube.New(this.GraphicsDevice, .5f, 8).ToMeshDraw() }, materialFree }
				)
			};

			debugcube.Transform.Position = new(x + .5f, 0, y + .5f);
			this.Entity.AddChild(debugcube);
		}

		this.grid.OnGridValueChanged += (_, args) =>
		{
			this.models[args.X, args.Y].Materials[0] = args.PathNode.IsWalkable ? materialFree : materialBlocked;
		};
	}

	public IEnumerable<Vector3> FindPath(Vector3 start, Vector3 end)
	{
		if (this.grid == null)
			return Array.Empty<Vector3>();

		start -= this.Entity.Transform.Position;
		end -= this.Entity.Transform.Position;

		return new Pathfinding(this.grid).FindPath((int)start.X, (int)start.Z, (int)end.X, (int)end.Z)
			.Select(pathNode => new Vector3(pathNode.X + .5f, start.Y, pathNode.Y + .5f) + this.Entity.Transform.Position);
	}

	public void ToggleWalkable(Vector3 origin)
	{
		if (this.grid == null)
			return;

		origin -= this.Entity.Transform.Position;

		this.grid.ToggleWalkable((int)origin.X, (int)origin.Z);
	}
}
