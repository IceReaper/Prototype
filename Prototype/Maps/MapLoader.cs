namespace Prototype.Maps;

using Entities;
using FileFormats;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Colors;
using Stride.Rendering.Lights;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;

public static class MapLoader
{
	public static void Load(Game game, Map map)
	{
		MapLoader.LoadSun(game, map);
		MapLoader.LoadBlocks(game, map);
		MapLoader.LoadLiquids(game, map);
		MapLoader.LoadLights(game, map);
	}

	private static void LoadSun(Game game, Map map)
	{
		var entity = Sun.Create(game);
		entity.Transform.Rotation = map.SunDirection;

		foreach (var lightComponent in entity.GetAll<LightComponent>())
		{
			switch (lightComponent.Type)
			{
				case LightAmbient lightAmbient:
					lightAmbient.Color = new ColorRgbProvider(map.SunAmbient);

					break;

				case LightDirectional lightDirectional:
					lightDirectional.Color = new ColorRgbProvider(map.SunDirectional);

					break;
			}
		}
	}

	private static void LoadBlocks(Game game, Map map)
	{
		var texture = TileSetBuilder.Build(game.GraphicsContext, map);
		var (vertexBuffer, indexBuffer) = Geometry.BuildBlocks(game.GraphicsDevice, map);

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(
			new()
			{
				new ModelComponent(
					new()
					{
						new Mesh
						{
							Draw = new() { VertexBuffers = new[] { vertexBuffer }, IndexBuffer = indexBuffer, DrawCount = indexBuffer.Count },
							BoundingBox = new(new(0, 0, 0), new(map.Cells.GetLength(0), map.Cells.GetLength(1), map.Cells.GetLength(2)))
						},
						new MaterialInstance(
							Material.New(
								game.GraphicsDevice,
								new()
								{
									Attributes =
									{
										Diffuse = new MaterialDiffuseMapFeature(
											new ComputeTextureColor(texture) { Filtering = TextureFilter.Point }
										),
										DiffuseModel = new MaterialDiffuseLambertModelFeature(),
										Transparency = new MaterialTransparencyCutoffFeature()
									}
								}
							)
						)
					}
				)
			}
		);
	}

	private static void LoadLiquids(Game game, Map map)
	{
		var (vertexBuffer, indexBuffer) = Geometry.BuildLiquids(game.GraphicsDevice, map);

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(
			new()
			{
				new ModelComponent(
					new()
					{
						new Mesh
						{
							Draw = new() { VertexBuffers = new[] { vertexBuffer }, IndexBuffer = indexBuffer, DrawCount = indexBuffer.Count },
							BoundingBox = new(new(0, 0, 0), new(map.Cells.GetLength(0), map.Cells.GetLength(1), map.Cells.GetLength(2)))
						},
						new MaterialInstance(
							Material.New(
								game.GraphicsDevice,
								new()
								{
									Attributes =
									{
										Diffuse = new MaterialDiffuseMapFeature(new ComputeVertexStreamColor()),
										DiffuseModel = new MaterialDiffuseLambertModelFeature()
									}
								}
							)
						)
					}
				)
			}
		);
	}

	private static void LoadLights(Game game, Map map)
	{
		foreach (var light in map.Lights)
		{
			var entity = PointLight.Create(game);
			entity.Transform.Position = light.Position;

			foreach (var lightComponent in entity.GetAll<LightComponent>())
			{
				lightComponent.Intensity = light.Intensity;

				if (lightComponent.Type is not LightPoint lightPoint)
					continue;

				lightPoint.Color = new ColorRgbProvider(light.Color);
				lightPoint.Radius = light.Radius;
			}
		}
	}
}
