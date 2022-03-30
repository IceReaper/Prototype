namespace Prototype;

using Entities.Components;
using Maps;
using Maps.FileFormats;
using Stride.Engine;
using Stride.Engine.Processors;

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

		camera.Transform.Position = new(117, 16, 215);

		this.SceneSystem.SceneInstance.RootScene.Entities.Add(camera);

		const string mapName = "bil";

		if (!File.Exists($"Assets/Maps/{mapName}.map") || true)
			LegacyMapConverter.Import(this.GraphicsContext, $"Assets/Maps/{mapName}", 608);

		MapLoader.Load(this.GraphicsContext, this.Services, Map.Read(File.OpenRead($"Assets/Maps/{mapName}.map")), this.SceneSystem.SceneInstance.RootScene);

		base.BeginRun();
	}
}
