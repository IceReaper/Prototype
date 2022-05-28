namespace Prototype.Systems.Maps.Shapes;

using Stride.Core.Mathematics;
using Stride.Graphics;

public sealed class SlopeEdgeShapeInner : Shape
{
	private static readonly VertexPositionNormalTexture[] ForwardLeft =
	{
		new(new(1, 1, 0), Vector3.Normalize(new(-1, 1, -1)), new(0, 0)),
		new(new(0, 1, 1), Vector3.Normalize(new(-1, 1, -1)), new(1, 0)),
		new(new(0, 0, 0), Vector3.Normalize(new(-1, 1, -1)), new(.5f, 1))
	};

	private static readonly VertexPositionNormalTexture[] ForwardRight =
	{
		new(new(1, 1, 1), Vector3.Normalize(new(1, 1, -1)), new(0, 0)),
		new(new(0, 1, 0), Vector3.Normalize(new(1, 1, -1)), new(1, 0)),
		new(new(1, 0, 0), Vector3.Normalize(new(1, 1, -1)), new(.5f, 1))
	};

	private static readonly VertexPositionNormalTexture[] BackwardLeft =
	{
		new(new(0, 1, 0), Vector3.Normalize(new(-1, 1, 1)), new(0, 0)),
		new(new(1, 1, 1), Vector3.Normalize(new(-1, 1, 1)), new(1, 0)),
		new(new(0, 0, 1), Vector3.Normalize(new(-1, 1, 1)), new(.5f, 1))
	};

	private static readonly VertexPositionNormalTexture[] BackwardRight =
	{
		new(new(0, 1, 1), Vector3.Normalize(new(1, 1, 1)), new(0, 0)),
		new(new(1, 1, 0), Vector3.Normalize(new(1, 1, 1)), new(1, 0)),
		new(new(1, 0, 1), Vector3.Normalize(new(1, 1, 1)), new(.5f, 1))
	};

	public SlopeEdgeShapeInner(float rotation)
	{
		(this.Forward, this.Backward, this.Up, this.Down, this.Left, this.Right) = rotation switch
		{
			3 => (Shape.FullForward,
				new[] { Shape.FullBackward[2], Shape.FullBackward[3], Shape.FullBackward[0], Shape.FullRight[1], Shape.FullRight[2], Shape.FullRight[3] },
				new[] { Shape.FullUp[0], Shape.FullUp[1], Shape.FullUp[3] }, Shape.FullDown, Shape.FullLeft, SlopeEdgeShapeInner.BackwardRight),
			2 => (Shape.FullForward,
				new[] { Shape.FullBackward[1], Shape.FullBackward[2], Shape.FullBackward[3], Shape.FullLeft[0], Shape.FullLeft[2], Shape.FullLeft[3] },
				new[] { Shape.FullUp[0], Shape.FullUp[1], Shape.FullUp[2] }, Shape.FullDown, SlopeEdgeShapeInner.BackwardLeft, Shape.FullRight),
			1 => (new[] { Shape.FullForward[1], Shape.FullForward[2], Shape.FullForward[3], Shape.FullRight[0], Shape.FullRight[2], Shape.FullRight[3] },
				Shape.FullBackward, new[] { Shape.FullUp[0], Shape.FullUp[2], Shape.FullUp[3] }, Shape.FullDown, Shape.FullLeft, SlopeEdgeShapeInner.ForwardRight),
			0 => (new[] { Shape.FullForward[2], Shape.FullForward[3], Shape.FullForward[0], Shape.FullLeft[1], Shape.FullLeft[2], Shape.FullLeft[3] },
				Shape.FullBackward, new[] { Shape.FullUp[1], Shape.FullUp[2], Shape.FullUp[3] }, Shape.FullDown, SlopeEdgeShapeInner.ForwardLeft, Shape.FullRight),
			_ => throw new InvalidOperationException()
		};
	}
}
