namespace Prototype.Systems.Entities;

using Scripts.EntityComponents;
using Stride.Engine;
using Utils;

public static class WorldGrid
{
	public static Entity Create(Game game)
	{
		SceneUtils.EnsureUnique(game, nameof(WorldGrid));

		var entity = new Entity(nameof(WorldGrid)) { new GridComponent() };

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
