namespace Prototype.Entities;

using Components;
using Stride.Engine;

public class UnitSelector
{
	public static Entity Create(Game game)
	{
		var entity = new Entity(nameof(SelectorComponent)) { new SelectorComponent() };

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
