namespace Prototype.Maps.FileFormats;

using Extensions;
using Stride.Core.Mathematics;

public class Light
{
	public Color Color;
	public Vector3 Position;
	public float Radius;
	public float Intensity;

	public static Light Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		return new() { Color = reader.ReadColor(), Position = reader.ReadVector3(), Radius = reader.ReadSingle(), Intensity = reader.ReadSingle() };
	}

	public static void Write(Stream stream, Light light)
	{
		var writer = new BinaryWriter(stream);

		writer.Write(light.Color);
		writer.Write(light.Position);
		writer.Write(light.Radius);
		writer.Write(light.Intensity);
	}
}
