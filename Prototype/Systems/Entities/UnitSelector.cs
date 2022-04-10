namespace Prototype.Systems.Entities;

using Scripts.EntityComponents;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;
using Utils;

public static class UnitSelector
{
	public static Entity Create(Game game)
	{
		SceneUtils.EnsureUnique(game, nameof(UnitSelector));

		var entity = new Entity(nameof(UnitSelector))
		{
			new ModelComponent { Model = new() { new Mesh { Draw = GeometricPrimitive.Cube.New(game.GraphicsDevice).ToMeshDraw() } } },
			new SelectorComponent()
		};

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
