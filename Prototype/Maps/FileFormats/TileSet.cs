namespace Prototype.Maps.FileFormats;

public class TileSet
{
	public readonly List<string> Tiles = new();

	public static TileSet Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		var tileSet = new TileSet();

		var numTiles = reader.ReadInt32();

		for (var i = 0; i < numTiles; i++)
			tileSet.Tiles.Add(reader.ReadString());

		return tileSet;
	}

	public static void Write(Stream stream, TileSet tileSet)
	{
		var writer = new BinaryWriter(stream);

		writer.Write(tileSet.Tiles.Count);

		foreach (var tile in tileSet.Tiles)
			writer.Write(tile);
	}
}
