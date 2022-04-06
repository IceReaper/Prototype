namespace Prototype;

using Entities;
using Maps;
using Maps.FileFormats;
using Stride.Engine;
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

		const string mapName = "bil";

		if (!File.Exists($"Assets/Maps/{mapName}.map") || true)
			LegacyMapConverter.Import(this.GraphicsContext, $"Assets/Maps/{mapName}", 608);

		MapLoader.Load(this, Map.Read(File.OpenRead($"Assets/Maps/{mapName}.map")));

		Camera.Create(this).Transform.Position = new(117, 16, 215);
		Cursor.Create(this);
		Character.Create(this).Transform.Position = new(119, 3, 210);
	}
}
