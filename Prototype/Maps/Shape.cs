namespace Prototype.Maps;

using FileFormats;
using Rendering.Depth;
using Stride.Graphics;

public abstract class Shape
{
	protected VertexPositionNormalTexture[] Forward = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Backward = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Up = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Down = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Left = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Right = Array.Empty<VertexPositionNormalTexture>();

	public IEnumerable<VertexPositionNormalTextureDepth> BuildBlock(Slice slice, Block block)
	{
		var vertices = new List<VertexPositionNormalTextureDepth>();

		vertices.AddRange(Geometry.Build(slice, this.Forward, block.Forward, false));
		vertices.AddRange(Geometry.Build(slice, this.Forward, block.ForwardInner, true));
		vertices.AddRange(Geometry.Build(slice, this.Backward, block.Backward, false));
		vertices.AddRange(Geometry.Build(slice, this.Backward, block.BackwardInner, true));
		vertices.AddRange(Geometry.Build(slice, this.Up, block.Up, false));
		vertices.AddRange(Geometry.Build(slice, this.Up, block.UpInner, true));
		vertices.AddRange(Geometry.Build(slice, this.Down, block.Down, false));
		vertices.AddRange(Geometry.Build(slice, this.Down, block.DownInner, true));
		vertices.AddRange(Geometry.Build(slice, this.Left, block.Left, false));
		vertices.AddRange(Geometry.Build(slice, this.Left, block.LeftInner, true));
		vertices.AddRange(Geometry.Build(slice, this.Right, block.Right, false));
		vertices.AddRange(Geometry.Build(slice, this.Right, block.RightInner, true));

		return vertices;
	}

	public IEnumerable<VertexPositionNormalColor> BuildLiquid(Liquid liquid)
	{
		return Geometry.Build(this.Up.Select(vertex => new VertexPositionNormalColor(vertex.Position, vertex.Normal, liquid.Color)));
	}
}
