namespace Prototype.Utils;

using Stride.Engine;

public static class SceneUtils
{
	public static void EnsureUnique(Game game, string type)
	{
		if (game.SceneSystem.SceneInstance.RootScene.Entities.Any(entity => entity.Name == type))
			throw new($"Entity of type <{type}> can only be added once to the scene!");
	}
}
