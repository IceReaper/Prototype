namespace Prototype.Entities;

using Prototype.Scripts.EntityComponents;
using Prototype.Utils;
using Stride.Engine;

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
