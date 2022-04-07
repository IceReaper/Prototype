namespace Prototype.Entities;

using Components;
using Stride.Engine;

public static class WorldGrid
{
	public static Entity Create(Game game)
	{
		var entity = new Entity(nameof(WorldGrid)) { new GridComponent() };

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
