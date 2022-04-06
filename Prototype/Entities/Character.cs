namespace Prototype.Entities;

using Components;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;

public static class Character
{
	public static Entity Create(Game game)
	{
		var entity = new Entity(nameof(Character))
		{
			new CharacterComponent(), new ModelComponent(new() { new Mesh { Draw = GeometricPrimitive.Capsule.New(game.GraphicsDevice).ToMeshDraw() } })
		};

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
