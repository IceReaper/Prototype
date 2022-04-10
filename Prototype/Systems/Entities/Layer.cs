namespace Prototype.Systems.Entities;

using Stride.Engine;

public static class Layer
{
	public static Entity Create(Game game)
	{
		var entity = new Entity(nameof(Layer));

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
