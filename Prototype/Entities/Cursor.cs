﻿namespace Prototype.Entities;

using Prototype.Scripts.EntityComponents;
using Prototype.Utils;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;

public static class Cursor
{
	public static Entity Create(Game game)
	{
		SceneUtils.EnsureUnique(game, nameof(Cursor));

		var entity = new Entity(nameof(Cursor))
		{
			new CursorComponent(),
			new ModelComponent { Model = new() { new Mesh { Draw = GeometricPrimitive.Sphere.New(game.GraphicsDevice, .25f).ToMeshDraw() } } }
		};

		game.SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

		return entity;
	}
}
