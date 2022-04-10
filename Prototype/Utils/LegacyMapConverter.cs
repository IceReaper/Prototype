namespace Prototype.Utils;

using Extensions;
using Stride.Core.Mathematics;
using Stride.Graphics;
using Systems.Maps.FileFormats;

public static class LegacyMapConverter
{
	private class TileUsage
	{
		public readonly ushort Tile;
		public readonly bool Transparent;

		public TileUsage(ushort tile, bool transparent)
		{
			this.Tile = tile;
			this.Transparent = transparent;
		}
	}

	private const string GmpMagic = "GBMP";
	private const string StyMagic = "GBST";
	private const string GeometryMagic = "DMAP";
	private const string LightMagic = "LGHT";
	private const string PaletteMappingsMagic = "PALX";
	private const string PaletteCatalogMagic = "PPAL";
	private const string TileCatalogMagic = "TILE";

	private const ushort GmpVersion = 500;
	private const ushort StyVersion = 700;

	private const int GeometryWidth = 256;
	private const int GeometryHeight = 256;
	private const int GeometryLayers = 8;

	private const int PaletteCatalogPageSize = 64;

	private const int TileCatalogTileSize = 64;
	private const int TileCatalogPageSize = 256;

	public static void Import(GraphicsContext graphicsContext, string path, int waterTile)
	{
		using var gmpStream = File.OpenRead($"{path}.gmp");
		using var styStream = File.OpenRead($"{path}.sty");

		var gmpReader = new BinaryReader(gmpStream);
		var styReader = new BinaryReader(styStream);

		if (gmpReader.ReadString(4) != LegacyMapConverter.GmpMagic)
			throw new($"Map magic is not {LegacyMapConverter.GmpMagic}");

		if (styReader.ReadString(4) != LegacyMapConverter.StyMagic)
			throw new($"SpriteBank magic is not {LegacyMapConverter.StyMagic}");

		if (gmpReader.ReadUInt16() != LegacyMapConverter.GmpVersion)
			throw new($"Map version is not {LegacyMapConverter.GmpVersion}");

		if (styReader.ReadUInt16() != LegacyMapConverter.StyVersion)
			throw new($"SpriteBank version is not {LegacyMapConverter.StyVersion}");

		var gmpChunks = new Dictionary<string, Stream>();
		var styChunks = new Dictionary<string, Stream>();

		while (gmpStream.Position < gmpStream.Length)
			gmpChunks.Add(gmpReader.ReadString(4), new MemoryStream(gmpReader.ReadBytes(gmpReader.ReadInt32())));

		while (styStream.Position < styStream.Length)
			styChunks.Add(styReader.ReadString(4), new MemoryStream(styReader.ReadBytes(styReader.ReadInt32())));

		if (!gmpChunks.TryGetValue(LegacyMapConverter.GeometryMagic, out var geometryStream))
			throw new($"Map is missing {LegacyMapConverter.GeometryMagic} chunk");

		if (!gmpChunks.TryGetValue(LegacyMapConverter.LightMagic, out var lightStream))
			throw new($"Map is missing {LegacyMapConverter.LightMagic} chunk");

		if (!styChunks.TryGetValue(LegacyMapConverter.PaletteMappingsMagic, out var paletteMappingsStream))
			throw new($"SpriteBank is missing {LegacyMapConverter.PaletteMappingsMagic} chunk");

		if (!styChunks.TryGetValue(LegacyMapConverter.PaletteCatalogMagic, out var paletteCatalogStream))
			throw new($"SpriteBank is missing {LegacyMapConverter.PaletteCatalogMagic} chunk");

		if (!styChunks.TryGetValue(LegacyMapConverter.TileCatalogMagic, out var tileCatalogStream))
			throw new($"SpriteBank is missing {LegacyMapConverter.TileCatalogMagic} chunk");

		var tileUsages = new List<TileUsage>();
		using var mapStream = File.Open($"{path}.map", FileMode.Create);

		Map.Write(
			mapStream,
			new()
			{
				SunAmbient = new(20, 40, 60),
				SunDirectional = new(40, 80, 120),
				SunDirection = Quaternion.RotationX(MathUtil.DegreesToRadians(-45.0f)) * Quaternion.RotationY(MathUtil.DegreesToRadians(135.0f)),
				Cells = LegacyMapConverter.ImportGeometry(geometryStream, tileUsages, waterTile),
				TileSet = LegacyMapConverter.ImportTileSet(
					graphicsContext,
					paletteMappingsStream,
					paletteCatalogStream,
					tileCatalogStream,
					tileUsages,
					Path.GetFileName(path)
				),
				Lights = LegacyMapConverter.ImportLights(lightStream).ToList()
			}
		);
	}

	private static Cell[,,] ImportGeometry(Stream stream, List<TileUsage> tileUsages, int waterTile)
	{
		var reader = new BinaryReader(stream);

		var cells = new Cell[LegacyMapConverter.GeometryWidth, LegacyMapConverter.GeometryLayers, LegacyMapConverter.GeometryHeight];

		for (var z = 0; z < cells.GetLength(2); z++)
		for (var y = 0; y < cells.GetLength(1); y++)
		for (var x = 0; x < cells.GetLength(0); x++)
			cells[x, y, z] = new();

		var columnOffsets = new uint[LegacyMapConverter.GeometryWidth, LegacyMapConverter.GeometryHeight];

		for (var x = 0; x < LegacyMapConverter.GeometryWidth; x++)
		for (var y = 0; y < LegacyMapConverter.GeometryHeight; y++)
			columnOffsets[x, y] = reader.ReadUInt32();

		var columns = new uint[reader.ReadUInt32()];

		for (var i = 0; i < columns.Length; i++)
			columns[i] = reader.ReadUInt32();

		var blocks = new Block?[reader.ReadUInt32()];
		var liquids = new Liquid?[blocks.Length];

		for (var i = 0; i < blocks.Length; i++)
			(blocks[i], liquids[i]) = LegacyMapConverter.ImportBlock(stream, tileUsages, waterTile);

		for (var x = 0; x < LegacyMapConverter.GeometryWidth; x++)
		for (var y = 0; y < LegacyMapConverter.GeometryHeight; y++)
		{
			var columnOffset = columnOffsets[x, y];
			var columnData = columns[columnOffset];
			var height = (byte)(columnData & 0x000000ff);
			var offset = (byte)((columnData & 0x0000ff00) >> 8);

			for (var layer = offset; layer < height; layer++)
			{
				var index = columns[columnOffset + layer - offset + 1];
				cells[y, layer, x].Block = blocks[index];
				cells[y, layer, x].Liquid = liquids[index];
			}
		}

		return cells;
	}

	private static (Block?, Liquid?) ImportBlock(Stream stream, List<TileUsage> tileUsages, int waterTile)
	{
		var reader = new BinaryReader(stream);

		var block = new Block
		{
			Left = LegacyMapConverter.ImportSide(stream, out var transparentLeft),
			Right = LegacyMapConverter.ImportSide(stream, out var transparentRight),
			Forward = LegacyMapConverter.ImportSide(stream, out var transparentForward),
			Backward = LegacyMapConverter.ImportSide(stream, out var transparentBackward),
			Up = LegacyMapConverter.ImportFront(stream, out var transparentUp)
		};

		var bitmask = reader.ReadUInt16();

		//// block.RoadLeft = (bitmask & 0x0001) != 0;
		//// block.RoadRight = (bitmask & 0x0002) != 0;
		//// block.RoadUp = (bitmask & 0x0004) != 0;
		//// block.RoadDown = (bitmask & 0x0008) != 0;

		//// block.SpecialLeft = (bitmask & 0x0010) != 0;
		//// block.SpecialRight = (bitmask & 0x0020) != 0;
		//// block.SpecialUp = (bitmask & 0x0040) != 0;
		//// block.SpecialDown = (bitmask & 0x0080) != 0;

		//// block.GroundType = (byte)((bitmask & 0x0300) >> 8);
		block.ShapeType = (byte)((bitmask & 0xfc00) >> 10);

		block = LegacyMapConverter.PatchBlock(
			block,
			tileUsages,
			transparentLeft,
			transparentRight,
			transparentForward,
			transparentBackward,
			transparentUp,
			waterTile,
			out var liquid
		);

		return (block, liquid);
	}

	private static Side? ImportSide(Stream stream, out bool transparent)
	{
		var reader = new BinaryReader(stream);

		var bitmask = reader.ReadUInt16();

		var tile = (ushort)(bitmask & 0x03ff);
		//// var blockEntities = (bitmask & 0x0400) != 0;
		//// var blockProjectiles = (bitmask & 0x0800) != 0;
		transparent = (bitmask & 0x1000) != 0;
		var flip = (bitmask & 0x2000) != 0;
		var rotation = (byte)((bitmask & 0xc000) >> 14);

		return tile == 0 ? null : new() { Material = tile, Flip = flip, Rotation = rotation };
	}

	private static Side? ImportFront(Stream stream, out bool transparent)
	{
		var reader = new BinaryReader(stream);

		var bitmask = reader.ReadUInt16();

		var tile = (ushort)(bitmask & 0x03ff);
		//// var light = (byte)((bitmask & 0x0c00) >> 10);
		transparent = (bitmask & 0x1000) != 0;
		var flip = (bitmask & 0x2000) != 0;
		var rotation = (byte)((bitmask & 0xc000) >> 14);

		return tile == 0 ? null : new() { Material = tile, Flip = flip, Rotation = rotation };
	}

	private static Block? PatchBlock(
		Block block,
		List<TileUsage> tileUsages,
		bool transparentLeft,
		bool transparentRight,
		bool transparentForward,
		bool transparentBackward,
		bool transparentUp,
		int waterTile,
		out Liquid? liquid
	)
	{
		if (transparentLeft)
		{
			if (block.Right != null)
				block.LeftInner = new() { Material = block.Right.Material, Flip = !block.Right.Flip, Rotation = block.Right.Rotation };

			if (!transparentRight)
				block.Right = null;
		}

		if (transparentRight)
		{
			if (block.Left != null)
				block.RightInner = new() { Material = block.Left.Material, Flip = !block.Left.Flip, Rotation = block.Left.Rotation };

			if (!transparentLeft)
				block.Left = null;
		}

		if (transparentForward)
		{
			if (block.Backward != null)
				block.ForwardInner = new() { Material = block.Backward.Material, Flip = !block.Backward.Flip, Rotation = block.Backward.Rotation };

			if (!transparentBackward)
				block.Backward = null;
		}

		if (transparentBackward)
		{
			if (block.Forward != null)
				block.BackwardInner = new() { Material = block.Forward.Material, Flip = !block.Forward.Flip, Rotation = block.Forward.Rotation };

			if (!transparentForward)
				block.Forward = null;
		}

		if (transparentUp)
		{
			if (block.Up != null)
				block.UpInner = new() { Material = block.Up.Material, Flip = block.Up.Flip, Rotation = block.Up.Rotation };
		}

		switch (block.ShapeType)
		{
			case >= 1 and <= 4 or >= 9 and <= 24 or >= 41 and <= 42:
				LegacyMapConverter.PatchSlopeSide(block.Left);
				LegacyMapConverter.PatchSlopeSide(block.Right);
				LegacyMapConverter.PatchSlopeSide(block.LeftInner);
				LegacyMapConverter.PatchSlopeSide(block.RightInner);

				break;

			case >= 5 and <= 8 or >= 25 and <= 40 or >= 43 and <= 44:
				LegacyMapConverter.PatchSlopeSide(block.Forward);
				LegacyMapConverter.PatchSlopeSide(block.Backward);
				LegacyMapConverter.PatchSlopeSide(block.ForwardInner);
				LegacyMapConverter.PatchSlopeSide(block.BackwardInner);

				break;

			case >= 49 and <= 52 when block.Up is { Material: 1023 }:
				block.ShapeType += 13;
				block.Up = null;
				block.UpInner = null;

				break;

			case 62 or 63:
				block.ShapeType = 0;

				break;
		}

		if (block.Up?.Material == waterTile)
		{
			liquid = new() { ShapeType = block.ShapeType, Color = Color.Aqua };
			block.Up = null;
		}
		else
			liquid = null;

		LegacyMapConverter.PatchMaterial(block.Left, tileUsages, transparentLeft);
		LegacyMapConverter.PatchMaterial(block.LeftInner, tileUsages, transparentLeft);
		LegacyMapConverter.PatchMaterial(block.Right, tileUsages, transparentRight);
		LegacyMapConverter.PatchMaterial(block.RightInner, tileUsages, transparentRight);
		LegacyMapConverter.PatchMaterial(block.Forward, tileUsages, transparentForward);
		LegacyMapConverter.PatchMaterial(block.ForwardInner, tileUsages, transparentForward);
		LegacyMapConverter.PatchMaterial(block.Backward, tileUsages, transparentBackward);
		LegacyMapConverter.PatchMaterial(block.BackwardInner, tileUsages, transparentBackward);
		LegacyMapConverter.PatchMaterial(block.Up, tileUsages, transparentUp);
		LegacyMapConverter.PatchMaterial(block.UpInner, tileUsages, transparentUp);

		return block is { Up: null, Left: null, Right: null, Forward: null, Backward: null } ? null : block;
	}

	private static void PatchSlopeSide(Side? side)
	{
		if (side == null)
			return;

		side.Flip = !side.Flip;
		side.Rotation = (byte)((side.Rotation + 2) % 4);
	}

	private static void PatchMaterial(Side? side, List<TileUsage> tileUsages, bool transparent)
	{
		if (side == null)
			return;

		var material = tileUsages.FindIndex(tileUsage => tileUsage.Tile == side.Material && tileUsage.Transparent == transparent);

		if (material == -1)
		{
			material = tileUsages.Count;
			tileUsages.Add(new(side.Material, transparent));
		}

		side.Material = (ushort)material;
	}

	private static TileSet ImportTileSet(
		GraphicsContext graphicsContext,
		Stream paletteMappingsStream,
		Stream paletteCatalogStream,
		Stream tileCatalogStream,
		IReadOnlyList<TileUsage> tileUsages,
		string name
	)
	{
		var paletteMappings = LegacyMapConverter.ReadPaletteMappings(paletteMappingsStream);
		var paletteCatalogs = new List<uint[][]>();
		var tileCatalogs = new List<byte[][]>();

		while (paletteCatalogStream.Position < paletteCatalogStream.Length)
			paletteCatalogs.Add(LegacyMapConverter.ReadPaletteCatalog(paletteCatalogStream));

		while (tileCatalogStream.Position < tileCatalogStream.Length)
			tileCatalogs.Add(LegacyMapConverter.ReadTileCatalog(tileCatalogStream));

		var tileCatalog = tileCatalogs.SelectMany(tileCatalog => tileCatalog).ToArray();

		for (var i = 0; i < tileUsages.Count; i++)
		{
			var path = $"Assets/Textures/{name}/{i}_diffuse.png";
			Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "");
			using var stream = File.Open(path, FileMode.Create);

			var texture = Texture.New2D(
				graphicsContext.CommandList.GraphicsDevice,
				LegacyMapConverter.TileCatalogTileSize,
				LegacyMapConverter.TileCatalogTileSize,
				PixelFormat.R8G8B8A8_UNorm_SRgb
			);

			texture.SetData(
				graphicsContext.CommandList,
				tileCatalog[tileUsages[i].Tile]
					.Select(index => index == 0 && tileUsages[i].Transparent ? 0 : paletteCatalogs[0][paletteMappings[tileUsages[i].Tile]][index])
					.ToArray()
			);

			texture.Save(graphicsContext.CommandList, stream, ImageFileType.Png);
		}

		var tileSet = new TileSet();
		tileSet.Tiles.AddRange(Enumerable.Range(0, tileUsages.Count).Select(i => $"{name}/{i}").ToArray());

		return tileSet;
	}

	private static IEnumerable<Light> ImportLights(Stream stream)
	{
		var reader = new BinaryReader(stream);

		var numLights = stream.Length / 16;

		for (var i = 0; i < numLights; i++)
		{
			var color = (reader.ReadUInt32Be() >> 8) | 0xff000000;
			var x = reader.ReadUInt16();
			var y = reader.ReadUInt16();
			var layer = reader.ReadUInt16();
			var radius = reader.ReadUInt16();
			var intensity = reader.ReadByte();
			reader.ReadBytes(3); // TimeRandom, TimeOn, TimeOff

			yield return new() { Color = new(color), Position = new Vector3(x, layer, y) / 128f, Radius = radius / 128f, Intensity = intensity / 255f };
		}
	}

	private static ushort[] ReadPaletteMappings(Stream stream)
	{
		var reader = new BinaryReader(stream);
		var paletteMappings = new ushort[stream.Length / 2];

		for (var i = 0; i < paletteMappings.Length; i++)
			paletteMappings[i] = reader.ReadUInt16();

		return paletteMappings;
	}

	private static uint[][] ReadPaletteCatalog(Stream stream)
	{
		var reader = new BinaryReader(stream);
		var paletteCatalog = new uint[LegacyMapConverter.PaletteCatalogPageSize][];

		for (var i = 0; i < paletteCatalog.Length; i++)
			paletteCatalog[i] = new uint[256];

		for (var i = 0; i < 256; i++)
		{
			for (var j = 0; j < LegacyMapConverter.PaletteCatalogPageSize; j++)
				paletteCatalog[j][i] = (reader.ReadUInt32Be() >> 8) | 0xff000000;
		}

		return paletteCatalog;
	}

	private static byte[][] ReadTileCatalog(Stream stream)
	{
		const int stride = LegacyMapConverter.TileCatalogPageSize / LegacyMapConverter.TileCatalogTileSize;

		var reader = new BinaryReader(stream);
		var tileCatalog = new byte[stride * stride][];

		for (var i = 0; i < tileCatalog.Length; i++)
			tileCatalog[i] = new byte[LegacyMapConverter.TileCatalogTileSize * LegacyMapConverter.TileCatalogTileSize];

		for (var tileY = 0; tileY < stride; tileY++)
		for (var y = 0; y < LegacyMapConverter.TileCatalogTileSize; y++)
		for (var tileX = 0; tileX < stride; tileX++)
		{
			Array.Copy(
				reader.ReadBytes(LegacyMapConverter.TileCatalogTileSize),
				0,
				tileCatalog[tileY * stride + tileX],
				y * LegacyMapConverter.TileCatalogTileSize,
				LegacyMapConverter.TileCatalogTileSize
			);
		}

		return tileCatalog;
	}
}
