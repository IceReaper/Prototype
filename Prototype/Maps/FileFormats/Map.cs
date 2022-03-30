namespace Prototype.Maps.FileFormats;

using Extensions;
using Stride.Core.Mathematics;

public class Map
{
	public Color SunAmbient;
	public Color SunDirectional;
	public Quaternion SunDirection;

	public readonly Dictionary<Vector3, Slice> Slices = new();

	public static Map Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		var map = new Map
		{
			SunAmbient = reader.ReadColor(),
			SunDirectional = reader.ReadColor(),
			SunDirection = reader.ReadQuaternion()
		};

		var numSlices = reader.ReadInt32();

		for (var i = 0; i < numSlices; i++)
			map.Slices.Add(reader.ReadVector3(), Slice.Read(stream));

		return map;
	}

	public static void Write(Stream stream, Map map)
	{
		var writer = new BinaryWriter(stream);

		writer.Write(map.SunAmbient);
		writer.Write(map.SunDirectional);
		writer.Write(map.SunDirection);
		writer.Write(map.Slices.Count);

		foreach (var (offset, slice) in map.Slices)
		{
			writer.Write(offset);
			Slice.Write(stream, slice);
		}
	}
}
