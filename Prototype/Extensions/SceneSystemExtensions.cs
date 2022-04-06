namespace Prototype.Extensions;

using Stride.Engine;

public static class SceneSystemExtensions
{
	public static IEnumerable<Entity> GetAll(this SceneSystem sceneSystem, string name)
	{
		return sceneSystem.SceneInstance.RootScene.Entities.Where(entity => entity.Name == name);
	}
}
