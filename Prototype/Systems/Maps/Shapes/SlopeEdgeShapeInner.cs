namespace Prototype.Systems.Maps.Shapes;

using Stride.Core.Mathematics;
using Stride.Graphics;

public class SlopeEdgeShapeInner : Shape
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
			3 => (Geometry.Forward,
				new[] { Geometry.Backward[2], Geometry.Backward[3], Geometry.Backward[0], Geometry.Right[1], Geometry.Right[2], Geometry.Right[3] },
				new[] { Geometry.Up[0], Geometry.Up[1], Geometry.Up[3] }, Geometry.Down, Geometry.Left, SlopeEdgeShapeInner.BackwardRight),
			2 => (Geometry.Forward,
				new[] { Geometry.Backward[1], Geometry.Backward[2], Geometry.Backward[3], Geometry.Left[0], Geometry.Left[2], Geometry.Left[3] },
				new[] { Geometry.Up[0], Geometry.Up[1], Geometry.Up[2] }, Geometry.Down, SlopeEdgeShapeInner.BackwardLeft, Geometry.Right),
			1 => (new[] { Geometry.Forward[1], Geometry.Forward[2], Geometry.Forward[3], Geometry.Right[0], Geometry.Right[2], Geometry.Right[3] },
				Geometry.Backward, new[] { Geometry.Up[0], Geometry.Up[2], Geometry.Up[3] }, Geometry.Down, Geometry.Left, SlopeEdgeShapeInner.ForwardRight),
			0 => (new[] { Geometry.Forward[2], Geometry.Forward[3], Geometry.Forward[0], Geometry.Left[1], Geometry.Left[2], Geometry.Left[3] },
				Geometry.Backward, new[] { Geometry.Up[1], Geometry.Up[2], Geometry.Up[3] }, Geometry.Down, SlopeEdgeShapeInner.ForwardLeft, Geometry.Right),
			_ => throw new()
		};
	}
}
