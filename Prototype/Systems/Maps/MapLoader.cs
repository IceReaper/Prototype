namespace Prototype.Systems.Maps;

using Entities;
using FileFormats;
using Stride.Engine;
using Stride.Games;
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
		var blockMaterial = MapLoader.LoadBlocksMaterial(game, map);
		var liquidsMaterial = MapLoader.LoadLiquidsMaterial(game);

		MapLoader.LoadSun(game, map);

		for (var y = 0; y < map.Cells.GetLength(1); y++)
		{
			MapLoader.LoadLayer(
				game,
				Geometry.BuildBlocks(game.GraphicsDevice, map, y, blockMaterial),
				Geometry.BuildLiquids(game.GraphicsDevice, map, y, liquidsMaterial),
				y
			);
		}

		foreach (var light in map.Lights)
			MapLoader.LoadLight(game, light);
	}

	private static MaterialInstance LoadBlocksMaterial(IGame game, Map map)
	{
		var texture = TileSetBuilder.Build(game.GraphicsContext, map);

		return new(
			Material.New(
				game.GraphicsDevice,
				new()
				{
					Attributes =
					{
						Diffuse = new MaterialDiffuseMapFeature(new ComputeTextureColor(texture) { Filtering = TextureFilter.Point }),
						DiffuseModel = new MaterialDiffuseLambertModelFeature(),
						Transparency = new MaterialTransparencyCutoffFeature()
					}
				}
			)
		);
	}

	private static MaterialInstance LoadLiquidsMaterial(IGame game)
	{
		return new(
			Material.New(
				game.GraphicsDevice,
				new()
				{
					Attributes =
					{
						Diffuse = new MaterialDiffuseMapFeature(new ComputeVertexStreamColor()), DiffuseModel = new MaterialDiffuseLambertModelFeature()
					}
				}
			)
		);
	}

	private static void LoadSun(Game game, Map map)
	{
		var entity = Sun.Create(game);
		entity.Transform.Rotation = map.SunDirection;

		foreach (var lightComponent in entity.Components.OfType<LightComponent>())
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

	private static void LoadLayer(Game game, Model? blocksModel, Model? liquidsModel, int layer)
	{
		var entity = Layer.Create(game);
		entity.Transform.Position.Y = layer;

		if (blocksModel == null && liquidsModel == null)
			return;

		var modelComponent = new ModelComponent { Model = new() { Children = new List<Model>() } };

		if (blocksModel != null)
			modelComponent.Model.Add(blocksModel);

		if (liquidsModel != null)
			modelComponent.Model.Add(blocksModel);

		modelComponent.Model = blocksModel;

		entity.Add(modelComponent);
	}

	private static void LoadLight(Game game, Light light)
	{
		var entity = PointLight.Create(game);
		entity.Transform.Position = light.Position;

		foreach (var lightComponent in entity.Components.OfType<LightComponent>())
		{
			lightComponent.Intensity = light.Intensity;

			if (lightComponent.Type is not LightPoint lightPoint)
				continue;

			lightPoint.Color = new ColorRgbProvider(light.Color);
			lightPoint.Radius = light.Radius;
		}
	}
}
