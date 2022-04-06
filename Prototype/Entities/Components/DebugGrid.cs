namespace Prototype.Entities.Components;

using Pathfinding;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;

public class DebugGrid : StartupScript
{
	private Grid? grid;

	public override void Start()
	{
		this.InitializeGrid();
	}

	private void InitializeGrid()
	{
		this.grid = new(20, 20, (g, x, y) => new(g, x, y));

		for (var x = 0; x < this.grid.Width; x++)
		for (var y = 0; y < this.grid.Height; y++)
		{
			var copyX = x;
			var copyY = y;

			var model = new Model { new Mesh { Draw = GeometricPrimitive.Cube.New(this.GraphicsDevice, .5f, 8).ToMeshDraw() } };

			var debugcube = new Entity { new ModelComponent(model) };
			debugcube.Transform.Position = new(x + .5f, 0, y + .5f);
			this.Entity.AddChild(debugcube);

			this.grid.OnGridValueChanged += (_, args) =>
			{
				if (args.X == copyX && args.Y == copyY)
				{
					var material = Material.New(
						this.GraphicsDevice,
						new()
						{
							Attributes =
							{
								Diffuse = new MaterialDiffuseMapFeature(new ComputeColor(args.PathNode.IsWalkable ? Color.White : Color.Red)), DiffuseModel = new MaterialDiffuseLambertModelFeature()
							}
						}
					);

					if (model.Materials.Count == 0)
						model.Add(material);
					else
						model.Materials[0] = material;
				}
			};
		}
	}

	public IEnumerable<Vector3> FindPath(Vector3 start, Vector3 end)
	{
		if (this.grid == null)
			return Array.Empty<Vector3>();

		start -= this.Entity.Transform.Position;
		end -= this.Entity.Transform.Position;
		var pathfinding = new Pathfinding(this.grid);

		return pathfinding.FindPath((int)start.X, (int)start.Z, (int)end.X, (int)end.Z)
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
