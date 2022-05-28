namespace Prototype.Systems.Maps.FileFormats;

using Utils;

public sealed class Side
{
	public ushort Material;
	public byte Rotation;
	public bool Flip;
	public bool IsHollow;

	public static Side Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		var side = new Side { Material = reader.ReadUInt16(), Rotation = reader.ReadByte() };

		var mask = reader.ReadByte();
		side.Flip = MaskUtils.Read(mask, 0);
		side.IsHollow = MaskUtils.Read(mask, 1);

		return side;
	}

	public static void Write(Stream stream, Side side)
	{
		var writer = new BinaryWriter(stream);

		writer.Write(side.Material);
		writer.Write(side.Rotation);

		byte mask = 0;

		MaskUtils.Write(ref mask, 0, side.Flip);
		MaskUtils.Write(ref mask, 1, side.IsHollow);

		writer.Write(mask);
	}
}
