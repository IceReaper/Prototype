namespace Prototype.Systems.Maps.Shapes;

using Stride.Core.Mathematics;
using Stride.Graphics;

public sealed class CubeEdgeShape : Shape
{
	private static readonly VertexPositionNormalTexture[] BackwardRight =
	{
		new(new(0, 1, 1), Vector3.Normalize(new(1, 0, 1)), new(0, 0)),
		new(new(1, 1, 0), Vector3.Normalize(new(1, 0, 1)), new(1, 0)),
		new(new(1, 0, 0), Vector3.Normalize(new(1, 0, 1)), new(1, 1)),
		new(new(0, 0, 1), Vector3.Normalize(new(1, 0, 1)), new(0, 1))
	};

	private static readonly VertexPositionNormalTexture[] BackwardLeft =
	{
		new(new(0, 1, 0), Vector3.Normalize(new(-1, 0, 1)), new(0, 0)),
		new(new(1, 1, 1), Vector3.Normalize(new(-1, 0, 1)), new(1, 0)),
		new(new(1, 0, 1), Vector3.Normalize(new(-1, 0, 1)), new(1, 1)),
		new(new(0, 0, 0), Vector3.Normalize(new(-1, 0, 1)), new(0, 1))
	};

	private static readonly VertexPositionNormalTexture[] ForwardRight =
	{
		new(new(1, 1, 1), Vector3.Normalize(new(1, 0, -1)), new(0, 0)),
		new(new(0, 1, 0), Vector3.Normalize(new(1, 0, -1)), new(1, 0)),
		new(new(0, 0, 0), Vector3.Normalize(new(1, 0, -1)), new(1, 1)),
		new(new(1, 0, 1), Vector3.Normalize(new(1, 0, -1)), new(0, 1))
	};

	private static readonly VertexPositionNormalTexture[] ForwardLeft =
	{
		new(new(1, 1, 0), Vector3.Normalize(new(-1, 0, -1)), new(0, 0)),
		new(new(0, 1, 1), Vector3.Normalize(new(-1, 0, -1)), new(1, 0)),
		new(new(0, 0, 1), Vector3.Normalize(new(-1, 0, -1)), new(1, 1)),
		new(new(1, 0, 0), Vector3.Normalize(new(-1, 0, -1)), new(0, 1))
	};

	public CubeEdgeShape(float rotation)
	{
		(this.Forward, this.Backward, this.Up, this.Down, this.Left, this.Right) = rotation switch
		{
			3 => (Shape.FullForward, Array.Empty<VertexPositionNormalTexture>(), new[] { Shape.FullUp[0], Shape.FullUp[1], Shape.FullUp[3] },
				new[] { Shape.FullDown[1], Shape.FullDown[2], Shape.FullDown[3] }, Shape.FullLeft, CubeEdgeShape.BackwardRight),
			2 => (Shape.FullForward, Array.Empty<VertexPositionNormalTexture>(), new[] { Shape.FullUp[0], Shape.FullUp[1], Shape.FullUp[2] },
				new[] { Shape.FullDown[0], Shape.FullDown[1], Shape.FullDown[2] }, CubeEdgeShape.BackwardLeft, Shape.FullRight),
			1 => (Array.Empty<VertexPositionNormalTexture>(), Shape.FullBackward, new[] { Shape.FullUp[0], Shape.FullUp[2], Shape.FullUp[3] },
				new[] { Shape.FullDown[0], Shape.FullDown[2], Shape.FullDown[3] }, Shape.FullLeft, CubeEdgeShape.ForwardRight),
			0 => (Array.Empty<VertexPositionNormalTexture>(), Shape.FullBackward, new[] { Shape.FullUp[1], Shape.FullUp[2], Shape.FullUp[3] },
				new[] { Shape.FullDown[0], Shape.FullDown[1], Shape.FullDown[3] }, CubeEdgeShape.ForwardLeft, Shape.FullRight),
			_ => throw new InvalidOperationException()
		};
	}
}
