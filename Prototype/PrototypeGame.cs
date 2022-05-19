namespace Prototype;

using Entities;
using Stride.Engine;
using Systems.Maps;
using Systems.Maps.FileFormats;
using Utils;

public sealed class PrototypeGame : Game
{
	protected override void BeginRun()
	{
		this.Window.AllowUserResizing = true;
		this.SceneSystem.GraphicsCompositor = GraphicsCompositorBuilder.Create();
		this.SceneSystem.SceneInstance = new(this.Services, new());

		if (!File.Exists("Assets/Maps/bil.map") || true)
			LegacyMapConverter.Import(this.GraphicsContext, "Assets/Maps/bil", 608);

		WorldGrid.Create(this);
		MapLoader.Load(this, Map.Read(File.OpenRead("Assets/Maps/bil.map")));
		Camera.Create(this).Transform.Position = new(117, 16, 215);
		Cursor.Create(this);
		UnitSelector.Create(this);

		Character.Create(this).Transform.Position = new(114.5f, 2, 210.5f);
		Character.Create(this).Transform.Position = new(122.5f, 2, 210.5f);
		Character.Create(this).Transform.Position = new(123.5f, 2, 210.5f);
		Character.Create(this).Transform.Position = new(124.5f, 2, 210.5f);
	}
}
