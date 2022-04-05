namespace Prototype.Components;

using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;
using System.Runtime.InteropServices;

public class WorldCursorComponent : SyncScript
{
	private Vector3 position;
	private CameraComponent? camera;
	private Entity? debugSphere;

	public Vector3 VirtualMousePosition;


	public override void Start()
	{
		this.camera = this.Entity.Components.OfType<CameraComponent>().FirstOrDefault();

		this.debugSphere = new()
		{
			new ModelComponent { Model = new() { new Mesh { Draw = GeometricPrimitive.Sphere.New(this.GraphicsDevice, .5f, 8).ToMeshDraw() } } }
		};

		this.SceneSystem.SceneInstance.RootScene.Entities.Add(this.debugSphere);
	}

	public override void Update()
	{
		if (this.camera == null)
			return;

		if (this.Input.HasMouse)
			this.position = WorldCursorComponent.GetWorldPosition(this.Input.MousePosition, this.camera);

		if (this.debugSphere != null)
		{
			this.debugSphere.Transform.Position = this.position;
			this.VirtualMousePosition = this.debugSphere.Transform.Position; //No direct Mouseinput, instead use virtualmouse.
		}
	}

	private static Vector3 GetWorldPosition(Vector2 mouse, CameraComponent camera)
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
