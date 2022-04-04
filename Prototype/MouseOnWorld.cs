namespace Prototype;

using ServiceWire.TcpIp;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;
using System.Runtime.CompilerServices;

public class MouseOnWorld : SyncScript
{
	private Vector3 mouseOnWorld;

	private CameraComponent? sceneCamera;

	private Entity? player; 
	
	public override void Start()
	{
		this.sceneCamera = this.Entity.Components.OfType<CameraComponent>().FirstOrDefault();
		this.CreateCapsule();
	}

	public override void Update()
	{
		if (this.sceneCamera == null)
			Console.Write("SceneCamera is null");

		if (this.Input.HasMouse)
			this.mouseOnWorld = this.GetLevelZero(this.Input.MousePosition, this.sceneCamera);

		Console.WriteLine(this.mouseOnWorld);
		Console.WriteLine(this.Input.MousePosition); // ATTENTION MACOS HAS SCALING UI

		if (this.player != null)
			this.player.Transform.Position = this.mouseOnWorld;
	}

	public void CreateCapsule()
	{
		this.player = new ();
		
		//Create a model and assign it to the model component
		var playermodel = new Model();
		this.player.GetOrCreate<ModelComponent>().Model = playermodel;

		var capsuleMeshDraw = GeometricPrimitive.Sphere.New(this.GraphicsDevice,.5f,8,1f,1f).ToMeshDraw();

		var mesh = new Mesh { Draw = capsuleMeshDraw };
		playermodel.Meshes.Add(mesh);

		this.player.Transform.Position = new(118, 2, 210);
		
		this.SceneSystem.SceneInstance.RootScene.Entities.Add(this.player);
	}

	private Vector3 GetLevelZero(Vector2 inputMousePosition, CameraComponent cameraComponent)
	{
		var plane = new Plane(new(0, -2, 0), new Vector3(0, 1, 0));
		var ray = this.ProjectMouse(inputMousePosition, cameraComponent);
		
		ray.Intersects(ref plane,out Vector3 location);

		return location;
	}


	public Ray ProjectMouse(Vector2 mousePosition, CameraComponent camera)
	{
		var inverseViewProjection = Matrix.Invert(camera.ViewProjectionMatrix);
		mousePosition = new(mousePosition.X * 2f - 1f, 1f - mousePosition.Y * 2f);
		var vector1 = Vector3.Transform(new(mousePosition, 0), inverseViewProjection);
		var vector2 = Vector3.Transform(new(mousePosition, 1), inverseViewProjection);

		var pos0 = (vector1 / vector1.W).XYZ();
		var pos1 = (vector2 / vector2.W).XYZ();

		var direction = Vector3.Normalize(pos1 - pos0);
		var position = pos0;
		
		return new(position, direction);
	}

}

