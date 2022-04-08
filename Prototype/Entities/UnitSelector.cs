namespace Prototype.Entities;

using Components;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;

public static class UnitSelector
{
	public static Entity Create(Game game)
	{
		var entity = new Entity(nameof(UnitSelector))
		{
			new ModelComponent { Model = new() { new Mesh { Draw = GeometricPrimitive.Cube.New(game.GraphicsDevice).ToMeshDraw() } } },
			new SelectorComponent()
		};

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
