namespace Prototype.Extensions;

using Stride.Core.Mathematics;

public static class BinaryWriterExtensions
{
	public static void Write(this BinaryWriter binaryWriter, Vector3 value)
	{
		binaryWriter.Write(value.X);
		binaryWriter.Write(value.Y);
		binaryWriter.Write(value.Z);
	}

	public static void Write(this BinaryWriter binaryWriter, Color value)
	{
		binaryWriter.Write(value.R);
		binaryWriter.Write(value.G);
		binaryWriter.Write(value.B);
		binaryWriter.Write(value.A);
	}

	public static void Write(this BinaryWriter binaryWriter, Quaternion value)
	{
		binaryWriter.Write(value.X);
		binaryWriter.Write(value.Y);
		binaryWriter.Write(value.Z);
		binaryWriter.Write(value.W);
	}
}
