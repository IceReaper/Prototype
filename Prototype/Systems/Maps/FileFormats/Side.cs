﻿namespace Prototype.Systems.Maps.FileFormats;

public sealed class Side
{
	public ushort Material;
	public bool Flip;
	public byte Rotation;
	public bool IsHollow;

	public static Side Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		return new() { Material = reader.ReadUInt16(), Flip = reader.ReadBoolean(), Rotation = reader.ReadByte() };
	}

	public static void Write(Stream stream, Side side)
	{
		var writer = new BinaryWriter(stream);

		writer.Write(side.Material);
		writer.Write(side.Flip);
		writer.Write(side.Rotation);
	}
}
