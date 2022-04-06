namespace Prototype.Maps.FileFormats;

using Extensions;
using Stride.Core.Mathematics;

public class Map
{
	public Color SunAmbient;
	public Color SunDirectional;
	public Quaternion SunDirection;

	public TileSet TileSet = new();
	public Cell[,,] Cells = new Cell[0, 0, 0];
	public List<Light> Lights = new();

	public static Map Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		var map = new Map
		{
			SunAmbient = reader.ReadColor(),
			SunDirectional = reader.ReadColor(),
			SunDirection = reader.ReadQuaternion(),
			TileSet = TileSet.Read(stream),
			Cells = new Cell[reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()]
		};

		for (var z = 0; z < map.Cells.GetLength(2); z++)
		for (var y = 0; y < map.Cells.GetLength(1); y++)
		for (var x = 0; x < map.Cells.GetLength(0); x++)
			map.Cells[x, y, z] = Cell.Read(stream);

		var numLights = reader.ReadInt32();

		for (var i = 0; i < numLights; i++)
			map.Lights.Add(Light.Read(stream));

		return map;
	}

	public static void Write(Stream stream, Map map)
	{
		var writer = new BinaryWriter(stream);

		writer.Write(map.SunAmbient);
		writer.Write(map.SunDirectional);
		writer.Write(map.SunDirection);

		TileSet.Write(stream, map.TileSet);

		writer.Write(map.Cells.GetLength(0));
		writer.Write(map.Cells.GetLength(1));
		writer.Write(map.Cells.GetLength(2));

		for (var z = 0; z < map.Cells.GetLength(2); z++)
		for (var y = 0; y < map.Cells.GetLength(1); y++)
		for (var x = 0; x < map.Cells.GetLength(0); x++)
			Cell.Write(stream, map.Cells[x, y, z]);

		writer.Write(map.Lights.Count);

		foreach (var light in map.Lights)
			Light.Write(stream, light);
	}
}
