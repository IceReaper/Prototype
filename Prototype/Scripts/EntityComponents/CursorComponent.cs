namespace Prototype.Scripts.EntityComponents;

using Activities;
using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using System.Runtime.InteropServices;
using Systems.Entities;

public class CursorComponent : SyncScript
{
	private CameraComponent? camera;

	private bool isClick;

	public override void Start()
	{
		this.camera = this.Entity.Scene.Entities.FirstOrDefault(nameof(Camera))?.Components.FirstOrDefault<CameraComponent>();
	}

	public override void Update()
	{
		if (this.camera == null)
			return;

		if (this.Input.HasMouse)
			this.Entity.Transform.Position = CursorComponent.GetWorldPosition(this.Input.MousePosition, this.camera);

		if (this.Input.IsMouseButtonPressed(MouseButton.Right))
			this.isClick = true;

		if (this.isClick
		    && (this.Input.MouseDelta * new Vector2(this.Game.Window.ClientBounds.Size.Width, this.Game.Window.ClientBounds.Size.Height)).Length() > 3)
			this.isClick = false;

		if (!this.Input.IsMouseButtonReleased(MouseButton.Right) || !this.isClick)
			return;

		this.isClick = false;

		// TODO this must be refactored into an order system via networking.
		foreach (var entity in this.Entity.Scene.Entities.OfType(nameof(Character)))
		{
			if (!(entity.Components.FirstOrDefault<CharacterComponent>()?.IsSelected ?? false))
				continue;

			var activitySystem = entity.Components.FirstOrDefault<ActivitySystemComponent>();

			if (activitySystem == null)
				continue;

			if (!this.Input.IsKeyPressed(Keys.LeftShift))
				activitySystem.Cancel();

			activitySystem.Add(new MoveActivity(entity, this.Entity.Transform.Position));
		}
	}

	private static Vector3 GetWorldPosition(Vector2 mouse, CameraComponent camera)
	{
		var plane = new Plane(new(0, -2, 0), new Vector3(0, 1, 0));

		CursorComponent.ProjectMouse(mouse, camera).Intersects(ref plane, out Vector3 location);

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
