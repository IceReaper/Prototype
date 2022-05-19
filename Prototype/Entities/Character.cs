namespace Prototype.Entities;

using Prototype.Scripts.EntityComponents;
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
			new ReserveCellComponent(),
			new CharacterComponent(),
			new ActivitySystemComponent(),

			// TODO capsule does not stand on y:0 -.-
			new ModelComponent(new() { new Mesh { Draw = GeometricPrimitive.Capsule.New(game.GraphicsDevice).ToMeshDraw() } })
		};

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
