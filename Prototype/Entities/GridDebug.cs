namespace Prototype.Pathfinding;

using Entities.Components;
using Stride.Engine;

public class GridDebug
{
	public static Entity Create(Game game)
	{
		var entity = new Entity(nameof(GridDebug))
		{
			new DebugGrid()
		};
		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);
		
		return entity;
	}
}
