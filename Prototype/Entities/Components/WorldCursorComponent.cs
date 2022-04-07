namespace Prototype.Entities.Components;

using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using System.Runtime.InteropServices;

public class WorldCursorComponent : SyncScript
{
	public override void Update()
	{
		var cameraEntity = this.SceneSystem.GetAll(nameof(Camera)).FirstOrDefault();
		var camera = cameraEntity?.Components.OfType<CameraComponent>().FirstOrDefault();

		if (camera == null)
			return;

		if (this.Input.HasMouse)
			this.Entity.Transform.Position = WorldCursorComponent.GetWorldPosition(this.Input.MousePosition, camera);

		// TODO this must be refactored into an order system.
		var characters = this.SceneSystem.GetAll(nameof(Character)).SelectMany(entity => entity.GetAll<CharacterComponent>()).ToArray();
		var debugGrid = this.SceneSystem.GetAll(nameof(WorldGrid)).SelectMany(entity => entity.GetAll<GridComponent>()).FirstOrDefault();

		if (this.Input.IsMouseButtonPressed(MouseButton.Left))
		{
			foreach (var character in characters)
			{
				if (debugGrid == null)
					character.Path.Add(this.Entity.Transform.Position);
				else
					character.Path.AddRange(debugGrid.FindPath(character.Entity.Transform.Position, this.Entity.Transform.Position));
			}
		}
		else if (this.Input.IsMouseButtonPressed(MouseButton.Right) && debugGrid != null)
			debugGrid.ToggleWalkable(this.Entity.Transform.Position);
	}

	public static Vector3 GetWorldPosition(Vector2 mouse, CameraComponent camera)
	{
		var plane = new Plane(new(0, -2, 0), new Vector3(0, 1, 0));

		WorldCursorComponent.ProjectMouse(mouse, camera).Intersects(ref plane, out Vector3 location);

		return location;
	}

	private static Ray ProjectMouse(Vector2 mousePosition, CameraComponent camera)
	{
		// TODO hack for OSX Retina "feature"!
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			mousePosition *= 2;

		var inverseViewProjection = Matrix.Invert(camera.ViewProjectionMatrix);
		mousePosition = new(mousePosition.X * 2f - 1f, 1f - mousePosition.Y * 2f);

		var vector1 = Vector3.Transform(new(mousePosition, 0), inverseViewProjection);
		var vector2 = Vector3.Transform(new(mousePosition, 1), inverseViewProjection);

		var pos1 = (vector1 / vector1.W).XYZ();
		var pos2 = (vector2 / vector2.W).XYZ();

		return new(pos1, Vector3.Normalize(pos2 - pos1));
	}
}
