namespace Prototype;

using Entities.Components;
using Maps;
using Maps.FileFormats;
using Stride.Engine;
using Stride.Engine.Processors;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;

public class PrototypeGame : Game
{
	protected override void BeginRun()
	{
		this.Window.AllowUserResizing = true;

		this.SceneSystem.GraphicsCompositor = GraphicsCompositorBuilder.Create();

		this.SceneSystem.SceneInstance = new(this.Services, new());

		var camera = new Entity
		{
			new CameraComponent { Projection = CameraProjectionMode.Perspective, Slot = this.SceneSystem.GraphicsCompositor.Cameras[0].ToSlotId() },
			new CameraControllerComponent()
		};
		camera.Add(new MouseOnWorld());
		camera.Transform.Position = new(117, 16, 215);

		this.SceneSystem.SceneInstance.RootScene.Entities.Add(camera);

		const string mapName = "bil";

		if (!File.Exists($"Assets/Maps/{mapName}.map") || true)
			LegacyMapConverter.Import(this.GraphicsContext, $"Assets/Maps/{mapName}", 608);

		MapLoader.Load(this.GraphicsContext, this.Services, Map.Read(File.OpenRead($"Assets/Maps/{mapName}.map")), this.SceneSystem.SceneInstance.RootScene);
		
		this.CreateCapsule();
		
		base.BeginRun();
	}

	public void CreateCapsule()
	{
		var player = new Entity();
		
		//Create a model and assign it to the model component
		var playermodel = new Model();
		player.GetOrCreate<ModelComponent>().Model = playermodel;

		var capsuleMeshDraw = GeometricPrimitive.Sphere.New(this.GraphicsDevice,1f,8,1f,1f).ToMeshDraw();

		var mesh = new Mesh { Draw = capsuleMeshDraw };
		playermodel.Meshes.Add(mesh);

		player.Transform.Position = new(119, 3, 210);
		
		this.SceneSystem.SceneInstance.RootScene.Entities.Add(player);
	}

}