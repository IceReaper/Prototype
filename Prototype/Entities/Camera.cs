namespace Prototype.Entities;

using Prototype.Scripts.EntityComponents;
using Prototype.Utils;
using Stride.Engine;
using Stride.Engine.Processors;

public static class Camera
{
	public static Entity Create(Game game)
	{
		SceneUtils.EnsureUnique(game, nameof(Camera));

		var entity = new Entity(nameof(Camera))
		{
			new CameraComponent { Projection = CameraProjectionMode.Perspective, Slot = game.SceneSystem.GraphicsCompositor.Cameras[0].ToSlotId() },
			new CameraControllerComponent()
		};

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
