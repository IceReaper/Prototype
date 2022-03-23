namespace Prototype.Maps.Shapes;

using Stride.Core.Mathematics;
using Stride.Graphics;

public class SlopeEdgeShapeOuter : Shape
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
			3 => (new[] { Geometry.Forward[1], Geometry.Forward[2], Geometry.Forward[3] }, Array.Empty<VertexPositionNormalTexture>(),
				Array.Empty<VertexPositionNormalTexture>(), new[] { Geometry.Down[1], Geometry.Down[2], Geometry.Down[3] },
				new[] { Geometry.Left[0], Geometry.Left[2], Geometry.Left[3] }, SlopeEdgeShapeOuter.BackwardRight),
			2 => (new[] { Geometry.Forward[0], Geometry.Forward[2], Geometry.Forward[3] }, Array.Empty<VertexPositionNormalTexture>(),
				Array.Empty<VertexPositionNormalTexture>(), new[] { Geometry.Down[0], Geometry.Down[1], Geometry.Down[2] }, SlopeEdgeShapeOuter.BackwardLeft,
				new[] { Geometry.Right[1], Geometry.Right[2], Geometry.Right[3] }),
			1 => (Array.Empty<VertexPositionNormalTexture>(), new[] { Geometry.Backward[0], Geometry.Backward[2], Geometry.Backward[3] },
				Array.Empty<VertexPositionNormalTexture>(), new[] { Geometry.Down[0], Geometry.Down[2], Geometry.Down[3] },
				new[] { Geometry.Left[1], Geometry.Left[2], Geometry.Left[3] }, SlopeEdgeShapeOuter.ForwardRight),
			0 => (Array.Empty<VertexPositionNormalTexture>(), new[] { Geometry.Backward[1], Geometry.Backward[2], Geometry.Backward[3] },
				Array.Empty<VertexPositionNormalTexture>(), new[] { Geometry.Down[0], Geometry.Down[1], Geometry.Down[3] }, SlopeEdgeShapeOuter.ForwardLeft,
				new[] { Geometry.Right[0], Geometry.Right[2], Geometry.Right[3] }),
			_ => throw new()
		};
	}
}
