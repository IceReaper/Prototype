namespace Prototype.Entities;

using Components;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;

public static class Cursor
{
	public static Entity Create(Game game)
	{
		var entity = new Entity(nameof(Cursor))
		{
			new CursorComponent(),
			new ModelComponent { Model = new() { new Mesh { Draw = GeometricPrimitive.Sphere.New(game.GraphicsDevice).ToMeshDraw() } } }
		};

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
