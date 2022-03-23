namespace Prototype.Extensions;

using Stride.Core.Mathematics;

public static class BinaryReaderExtensions
{
	public static Vector3 ReadVector3(this BinaryReader binaryReader)
	{
		return new(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
	}

	public static Color ReadColor(this BinaryReader binaryReader)
	{
		return new(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
	}

	public static Quaternion ReadQuaternion(this BinaryReader binaryReader)
	{
		return new(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
	}

	public static string ReadString(this BinaryReader binaryReader, int length)
	{
		return new(binaryReader.ReadChars(length));
	}

	public static uint ReadUInt32Be(this BinaryReader binaryReader)
	{
		return BitConverter.ToUInt32(binaryReader.ReadBytes(4).Reverse().ToArray());
	}
}
