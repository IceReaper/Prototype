namespace Prototype.Maps;

using FileFormats;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Colors;
using Stride.Rendering.Lights;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;

public static class MapLoader
{
	public static void Load(GraphicsContext graphicsContext, ServiceRegistry serviceRegistry, Map map, Scene scene)
	{
		foreach (var entity in scene.Entities.Where(entity => !entity.Components.GetAll<CameraComponent>().Any()))
			entity.Dispose();

		MapLoader.LoadSun(map, scene);

		foreach (var (offset, slice) in map.Slices)
		{
			MapLoader.LoadBlocks(graphicsContext, slice, scene, offset);
			MapLoader.LoadLiquids(graphicsContext, slice, scene, offset);
			MapLoader.LoadLights(slice, scene, offset);
		}
	}

	private static void LoadSun(Map map, Scene scene)
	{
		scene.Entities.Add(new() { new LightComponent { Type = new LightAmbient { Color = new ColorRgbProvider(map.SunAmbient) } } });

		var entity = new Entity
		{
			new LightComponent
			{
				Type = new LightDirectional
				{
					Color = new ColorRgbProvider(map.SunDirectional),
					Shadow =
					{
						Enabled = true,
						Size = LightShadowMapSize.XLarge,
						Filter = new LightShadowMapFilterTypePcf { FilterSize = LightShadowMapFilterTypePcfSize.Filter7x7 }
					}
				}
			}
		};

		entity.Transform.Rotation = map.SunDirection;
		scene.Entities.Add(entity);
	}

	private static void LoadBlocks(GraphicsContext graphicsContext, Slice slice, Scene scene, Vector3 offset)
	{
		var texture = TileSetBuilder.Build(graphicsContext, slice);
		var (vertexBuffer, indexBuffer) = Geometry.BuildBlocks(graphicsContext.CommandList.GraphicsDevice, slice, offset);

		scene.Entities.Add(
			new()
			{
				new ModelComponent(
					new()
					{
						new Mesh
						{
							Draw = new() { VertexBuffers = new[] { vertexBuffer }, IndexBuffer = indexBuffer, DrawCount = indexBuffer.Count },
							BoundingBox = new(new(0, 0, 0), new(slice.Cells.GetLength(0), slice.Cells.GetLength(1), slice.Cells.GetLength(2)))
						},
						new MaterialInstance(
							Material.New(
								graphicsContext.CommandList.GraphicsDevice,
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

	private static void LoadLiquids(GraphicsContext graphicsContext, Slice slice, Scene scene, Vector3 offset)
	{
		var (vertexBuffer, indexBuffer) = Geometry.BuildLiquids(graphicsContext.CommandList.GraphicsDevice, slice, offset);

		scene.Entities.Add(
			new()
			{
				new ModelComponent(
					new()
					{
						new Mesh
						{
							Draw = new() { VertexBuffers = new[] { vertexBuffer }, IndexBuffer = indexBuffer, DrawCount = indexBuffer.Count },
							BoundingBox = new(new(0, 0, 0), new(slice.Cells.GetLength(0), slice.Cells.GetLength(1), slice.Cells.GetLength(2)))
						},
						new MaterialInstance(
							Material.New(
								graphicsContext.CommandList.GraphicsDevice,
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

	private static void LoadLights(Slice slice, Scene scene, Vector3 offset)
	{
		foreach (var light in slice.Lights)
		{
			var entity = new Entity
			{
				new LightComponent
				{
					Intensity = light.Intensity, Type = new LightPoint { Color = new ColorRgbProvider(light.Color), Radius = light.Radius }
				}
			};

			entity.Transform.Position = offset + light.Position;
			scene.Entities.Add(entity);
		}
	}
}
