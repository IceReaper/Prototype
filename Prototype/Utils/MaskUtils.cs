namespace Prototype.Utils;

public static class MaskUtils
{
	public static bool Read(byte mask, int index)
	{
		return ((mask >> index) & 1) != 0;
	}

	public static void Write(ref byte mask, int index, bool value)
	{
		if (value)
			mask |= (byte)(1 << index);
	}
}
