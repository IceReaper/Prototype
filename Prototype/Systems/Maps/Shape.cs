namespace Prototype.Systems.Maps;

using DepthRendering;
using FileFormats;
using Stride.Graphics;

public abstract class Shape
{
	protected static readonly VertexPositionNormalTexture[] FullRight =
	{
		new(new(1, 1, 1), new(1, 0, 0), new(0, 0)),
		new(new(1, 1, 0), new(1, 0, 0), new(1, 0)),
		new(new(1, 0, 0), new(1, 0, 0), new(1, 1)),
		new(new(1, 0, 1), new(1, 0, 0), new(0, 1))
	};

	protected static readonly VertexPositionNormalTexture[] FullLeft =
	{
		new(new(0, 1, 0), new(-1, 0, 0), new(0, 0)),
		new(new(0, 1, 1), new(-1, 0, 0), new(1, 0)),
		new(new(0, 0, 1), new(-1, 0, 0), new(1, 1)),
		new(new(0, 0, 0), new(-1, 0, 0), new(0, 1))
	};

	protected static readonly VertexPositionNormalTexture[] FullUp =
	{
		new(new(0, 1, 0), new(0, 1, 0), new(0, 0)),
		new(new(1, 1, 0), new(0, 1, 0), new(1, 0)),
		new(new(1, 1, 1), new(0, 1, 0), new(1, 1)),
		new(new(0, 1, 1), new(0, 1, 0), new(0, 1))
	};

	protected static readonly VertexPositionNormalTexture[] FullDown =
	{
		new(new(1, 0, 1), new(0, -1, 0), new(1, 1)),
		new(new(1, 0, 0), new(0, -1, 0), new(1, 0)),
		new(new(0, 0, 0), new(0, -1, 0), new(0, 0)),
		new(new(0, 0, 1), new(0, -1, 0), new(0, 1))
	};

	protected static readonly VertexPositionNormalTexture[] FullBackward =
	{
		new(new(0, 1, 1), new(0, 0, 1), new(0, 0)),
		new(new(1, 1, 1), new(0, 0, 1), new(1, 0)),
		new(new(1, 0, 1), new(0, 0, 1), new(1, 1)),
		new(new(0, 0, 1), new(0, 0, 1), new(0, 1))
	};

	protected static readonly VertexPositionNormalTexture[] FullForward =
	{
		new(new(1, 1, 0), new(0, 0, -1), new(0, 0)),
		new(new(0, 1, 0), new(0, 0, -1), new(1, 0)),
		new(new(0, 0, 0), new(0, 0, -1), new(1, 1)),
		new(new(1, 0, 0), new(0, 0, -1), new(0, 1))
	};

	protected VertexPositionNormalTexture[] Forward = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Backward = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Up = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Down = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Left = Array.Empty<VertexPositionNormalTexture>();
	protected VertexPositionNormalTexture[] Right = Array.Empty<VertexPositionNormalTexture>();

	public IEnumerable<VertexPositionNormalTextureDepth> BuildBlock(Map map, Block block)
	{
		var vertices = new List<VertexPositionNormalTextureDepth>();

		vertices.AddRange(Geometry.Build(map, this.Forward, block.Forward, false));
		vertices.AddRange(Geometry.Build(map, this.Forward, block.ForwardInner, true));
		vertices.AddRange(Geometry.Build(map, this.Backward, block.Backward, false));
		vertices.AddRange(Geometry.Build(map, this.Backward, block.BackwardInner, true));
		vertices.AddRange(Geometry.Build(map, this.Up, block.Up, false));
		vertices.AddRange(Geometry.Build(map, this.Up, block.UpInner, true));
		vertices.AddRange(Geometry.Build(map, this.Down, block.Down, false));
		vertices.AddRange(Geometry.Build(map, this.Down, block.DownInner, true));
		vertices.AddRange(Geometry.Build(map, this.Left, block.Left, false));
		vertices.AddRange(Geometry.Build(map, this.Left, block.LeftInner, true));
		vertices.AddRange(Geometry.Build(map, this.Right, block.Right, false));
		vertices.AddRange(Geometry.Build(map, this.Right, block.RightInner, true));

		return vertices;
	}

	public IEnumerable<VertexPositionNormalColor> BuildLiquid(Liquid liquid)
	{
		return Geometry.Build(this.Up.Select(vertex => new VertexPositionNormalColor(vertex.Position, vertex.Normal, liquid.Color)));
	}
}
