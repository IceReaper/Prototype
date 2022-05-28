namespace Prototype.Systems.Maps.FileFormats;

using Utils;

public sealed class Block
{
	public byte ShapeType;

	public Side? Forward;
	public Side? Backward;
	public Side? Up;
	public Side? Down;
	public Side? Left;
	public Side? Right;

	public Side? ForwardInner;
	public Side? BackwardInner;
	public Side? UpInner;
	public Side? DownInner;
	public Side? LeftInner;
	public Side? RightInner;

	public static Block Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		var block = new Block { ShapeType = reader.ReadByte() };

		var outerMask = reader.ReadByte();

		block.Forward = Block.Read(stream, outerMask, 0);
		block.Backward = Block.Read(stream, outerMask, 1);
		block.Up = Block.Read(stream, outerMask, 2);
		block.Down = Block.Read(stream, outerMask, 3);
		block.Left = Block.Read(stream, outerMask, 4);
		block.Right = Block.Read(stream, outerMask, 5);

		var innerMask = reader.ReadByte();

		block.ForwardInner = Block.Read(stream, innerMask, 0);
		block.BackwardInner = Block.Read(stream, innerMask, 1);
		block.UpInner = Block.Read(stream, innerMask, 2);
		block.DownInner = Block.Read(stream, innerMask, 3);
		block.LeftInner = Block.Read(stream, innerMask, 4);
		block.RightInner = Block.Read(stream, innerMask, 5);

		return block;
	}

	private static Side? Read(Stream stream, byte mask, int index)
	{
		return MaskUtils.Read(mask, index) ? Side.Read(stream) : null;
	}

	public static void Write(Stream stream, Block block)
	{
		var writer = new BinaryWriter(stream);

		writer.Write(block.ShapeType);

		byte outerMask = 0;
		MaskUtils.Write(ref outerMask, 0, block.Forward != null);
		MaskUtils.Write(ref outerMask, 1, block.Backward != null);
		MaskUtils.Write(ref outerMask, 2, block.Up != null);
		MaskUtils.Write(ref outerMask, 3, block.Down != null);
		MaskUtils.Write(ref outerMask, 4, block.Left != null);
		MaskUtils.Write(ref outerMask, 5, block.Right != null);
		writer.Write(outerMask);

		Block.Write(stream, block.Forward);
		Block.Write(stream, block.Backward);
		Block.Write(stream, block.Up);
		Block.Write(stream, block.Down);
		Block.Write(stream, block.Left);
		Block.Write(stream, block.Right);

		byte innerMask = 0;
		MaskUtils.Write(ref innerMask, 0, block.ForwardInner != null);
		MaskUtils.Write(ref innerMask, 1, block.BackwardInner != null);
		MaskUtils.Write(ref innerMask, 2, block.UpInner != null);
		MaskUtils.Write(ref innerMask, 3, block.DownInner != null);
		MaskUtils.Write(ref innerMask, 4, block.LeftInner != null);
		MaskUtils.Write(ref innerMask, 5, block.RightInner != null);
		writer.Write(innerMask);

		Block.Write(stream, block.ForwardInner);
		Block.Write(stream, block.BackwardInner);
		Block.Write(stream, block.UpInner);
		Block.Write(stream, block.DownInner);
		Block.Write(stream, block.LeftInner);
		Block.Write(stream, block.RightInner);
	}

	private static void Write(Stream stream, Side? side)
	{
		if (side != null)
			Side.Write(stream, side);
	}
}
