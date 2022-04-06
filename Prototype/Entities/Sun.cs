namespace Prototype.Entities;

using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering.Colors;
using Stride.Rendering.Lights;

public static class Sun
{
	public static Entity Create(Game game)
	{
		var entity = new Entity(nameof(Sun))
		{
			new LightComponent { Type = new LightAmbient { Color = new ColorRgbProvider(Color.White) } },
			new LightComponent
			{
				Type = new LightDirectional
				{
					Color = new ColorRgbProvider(Color.White),
					Shadow =
					{
						Enabled = true,
						Size = LightShadowMapSize.XLarge,
						Filter = new LightShadowMapFilterTypePcf { FilterSize = LightShadowMapFilterTypePcfSize.Filter7x7 }
					}
				}
			}
		};

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
