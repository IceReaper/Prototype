namespace Prototype.Maps.FileFormats;

using Utils;

public class Cell
{
	public Block? Block;
	public Liquid? Liquid;

	public static Cell Read(Stream stream)
	{
		var reader = new BinaryReader(stream);

		var cell = new Cell();

		var mask = reader.ReadByte();

		if (MaskUtils.ReadBool(mask, 0))
			cell.Block = Block.Read(stream);

		if (MaskUtils.ReadBool(mask, 1))
			cell.Liquid = Liquid.Read(stream);

		return cell;
	}

	public static void Write(Stream stream, Cell cell)
	{
		var writer = new BinaryWriter(stream);

		byte mask = 0;
		MaskUtils.WriteNotNull(ref mask, 0, cell.Block);
		MaskUtils.WriteNotNull(ref mask, 1, cell.Liquid);
		writer.Write(mask);

		if (cell.Block != null)
			Block.Write(stream, cell.Block);

		if (cell.Liquid != null)
			Liquid.Write(stream, cell.Liquid);
	}
}
