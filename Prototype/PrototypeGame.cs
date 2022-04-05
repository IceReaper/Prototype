namespace Prototype;

using Components;
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

		var camera = new Entity()
		{
			new CameraComponent { Projection = CameraProjectionMode.Perspective, Slot = this.SceneSystem.GraphicsCompositor.Cameras[0].ToSlotId() },
			new CameraControllerComponent(),
			new WorldCursorComponent()
		};

		camera.Transform.Position = new(117, 16, 215);

		this.SceneSystem.SceneInstance.RootScene.Entities.Add(camera);

		const string mapName = "bil";

		if (!File.Exists($"Assets/Maps/{mapName}.map") || true)
			LegacyMapConverter.Import(this.GraphicsContext, $"Assets/Maps/{mapName}", 608);

		MapLoader.Load(this.GraphicsContext, this.Services, Map.Read(File.OpenRead($"Assets/Maps/{mapName}.map")), this.SceneSystem.SceneInstance.RootScene);

		var character = new Entity { new ModelComponent(new() { new Mesh { Draw = GeometricPrimitive.Sphere.New(this.GraphicsDevice, 1f, 8).ToMeshDraw() } }) };
		character.Transform.Position = new(119, 3, 210);
		this.SceneSystem.SceneInstance.RootScene.Entities.Add(character);

		this.CreatePlayer();
		base.BeginRun();
	}
	
	public void CreatePlayer()
	{
		var player = new Entity();
		player.Add(new Player());
		player.Transform.Position = new(119, 3, 211);
		this.SceneSystem.SceneInstance.RootScene.Entities.Add(player);

	}
}
