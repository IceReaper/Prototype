namespace Prototype.Systems.Entities;

using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering.Colors;
using Stride.Rendering.Lights;

public static class PointLight
{
	public static Entity Create(Game game)
	{
		var entity = new Entity(nameof(PointLight))
		{
			new LightComponent { Intensity = 1, Type = new LightPoint { Color = new ColorRgbProvider(Color.White), Radius = 1 } }
		};

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
