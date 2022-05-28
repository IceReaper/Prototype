namespace Prototype.Systems.Maps.Shapes;

using Stride.Core.Mathematics;
using Stride.Graphics;

public sealed class SlopeEdgeShapeOuter : Shape
{
	private static readonly VertexPositionNormalTexture[] ForwardLeft =
	{
		new(new(1, 1, 1), Vector3.Normalize(new(-1, 1, -1)), new(.5f, 0)),
		new(new(0, 0, 1), Vector3.Normalize(new(-1, 1, -1)), new(1, 1)),
		new(new(1, 0, 0), Vector3.Normalize(new(-1, 1, -1)), new(0, 1))
	};

	private static readonly VertexPositionNormalTexture[] ForwardRight =
	{
		new(new(0, 1, 1), Vector3.Normalize(new(1, 1, -1)), new(.5f, 0)),
		new(new(0, 0, 0), Vector3.Normalize(new(1, 1, -1)), new(1, 1)),
		new(new(1, 0, 1), Vector3.Normalize(new(1, 1, -1)), new(0, 1))
	};

	private static readonly VertexPositionNormalTexture[] BackwardLeft =
	{
		new(new(1, 1, 0), Vector3.Normalize(new(-1, 1, 1)), new(.5f, 0)),
		new(new(1, 0, 1), Vector3.Normalize(new(-1, 1, 1)), new(1, 1)),
		new(new(0, 0, 0), Vector3.Normalize(new(-1, 1, 1)), new(0, 1))
	};

	private static readonly VertexPositionNormalTexture[] BackwardRight =
	{
		new(new(0, 1, 0), Vector3.Normalize(new(1, 1, 1)), new(.5f, 0)),
		new(new(1, 0, 0), Vector3.Normalize(new(1, 1, 1)), new(1, 1)),
		new(new(0, 0, 1), Vector3.Normalize(new(1, 1, 1)), new(0, 1))
	};

	public SlopeEdgeShapeOuter(float rotation)
	{
		(this.Forward, this.Backward, this.Up, this.Down, this.Left, this.Right) = rotation switch
		{
			3 => (new[] { Shape.FullForward[1], Shape.FullForward[2], Shape.FullForward[3] }, Array.Empty<VertexPositionNormalTexture>(),
				Array.Empty<VertexPositionNormalTexture>(), new[] { Shape.FullDown[1], Shape.FullDown[2], Shape.FullDown[3] },
				new[] { Shape.FullLeft[0], Shape.FullLeft[2], Shape.FullLeft[3] }, SlopeEdgeShapeOuter.BackwardRight),
			2 => (new[] { Shape.FullForward[0], Shape.FullForward[2], Shape.FullForward[3] }, Array.Empty<VertexPositionNormalTexture>(),
				Array.Empty<VertexPositionNormalTexture>(), new[] { Shape.FullDown[0], Shape.FullDown[1], Shape.FullDown[2] }, SlopeEdgeShapeOuter.BackwardLeft,
				new[] { Shape.FullRight[1], Shape.FullRight[2], Shape.FullRight[3] }),
			1 => (Array.Empty<VertexPositionNormalTexture>(), new[] { Shape.FullBackward[0], Shape.FullBackward[2], Shape.FullBackward[3] },
				Array.Empty<VertexPositionNormalTexture>(), new[] { Shape.FullDown[0], Shape.FullDown[2], Shape.FullDown[3] },
				new[] { Shape.FullLeft[1], Shape.FullLeft[2], Shape.FullLeft[3] }, SlopeEdgeShapeOuter.ForwardRight),
			0 => (Array.Empty<VertexPositionNormalTexture>(), new[] { Shape.FullBackward[1], Shape.FullBackward[2], Shape.FullBackward[3] },
				Array.Empty<VertexPositionNormalTexture>(), new[] { Shape.FullDown[0], Shape.FullDown[1], Shape.FullDown[3] }, SlopeEdgeShapeOuter.ForwardLeft,
				new[] { Shape.FullRight[0], Shape.FullRight[2], Shape.FullRight[3] }),
			_ => throw new InvalidOperationException()
		};
	}
}
