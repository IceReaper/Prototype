namespace Prototype.Utils;

public static class MaskUtils
{
	public static void WriteNotNull(ref byte mask, int index, object? value)
	{
		if (value != null)
			mask |= (byte)(1 << index);
	}

	public static bool ReadBool(byte mask, int index)
	{
		return ((mask >> index) & 1) != 0;
	}
}
