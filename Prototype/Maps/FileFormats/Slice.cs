namespace Prototype.Maps.FileFormats;

public class Slice
{
	public TileSet TileSet = new();
	public Cell[,,] Cells = new Cell[0, 0, 0];
	public readonly List<Light> Lights = new();

	public static Slice Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		var slice = new Slice { TileSet = TileSet.Read(stream), Cells = new Cell[reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()] };

		for (var z = 0; z < slice.Cells.GetLength(2); z++)
		for (var y = 0; y < slice.Cells.GetLength(1); y++)
		for (var x = 0; x < slice.Cells.GetLength(0); x++)
			slice.Cells[x, y, z] = Cell.Read(stream);

		var numLights = reader.ReadInt32();

		for (var i = 0; i < numLights; i++)
			slice.Lights.Add(Light.Read(stream));

		return slice;
	}

	public static void Write(Stream stream, Slice slice)
	{
		var writer = new BinaryWriter(stream);

		TileSet.Write(stream, slice.TileSet);

		writer.Write(slice.Cells.GetLength(0));
		writer.Write(slice.Cells.GetLength(1));
		writer.Write(slice.Cells.GetLength(2));

		for (var z = 0; z < slice.Cells.GetLength(2); z++)
		for (var y = 0; y < slice.Cells.GetLength(1); y++)
		for (var x = 0; x < slice.Cells.GetLength(0); x++)
			Cell.Write(stream, slice.Cells[x, y, z]);

		writer.Write(slice.Lights.Count);

		foreach (var light in slice.Lights)
			Light.Write(stream, light);
	}
}
