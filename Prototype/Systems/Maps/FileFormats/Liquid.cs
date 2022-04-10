namespace Prototype.Systems.Maps.FileFormats;

using Extensions;
using Stride.Core.Mathematics;

public class Liquid
{
	public byte ShapeType;
	public Color Color;

	public static Liquid Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		return new() { ShapeType = reader.ReadByte(), Color = reader.ReadColor() };
	}

	public static void Write(Stream stream, Liquid liquid)
	{
		var writer = new BinaryWriter(stream);

		writer.Write(liquid.ShapeType);
		writer.Write(liquid.Color);
	}
}
