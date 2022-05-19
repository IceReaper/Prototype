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
			3 => (Geometry.Forward, Array.Empty<VertexPositionNormalTexture>(), new[] { Geometry.Up[0], Geometry.Up[1], Geometry.Up[3] },
				new[] { Geometry.Down[1], Geometry.Down[2], Geometry.Down[3] }, Geometry.Left, CubeEdgeShape.BackwardRight),
			2 => (Geometry.Forward, Array.Empty<VertexPositionNormalTexture>(), new[] { Geometry.Up[0], Geometry.Up[1], Geometry.Up[2] },
				new[] { Geometry.Down[0], Geometry.Down[1], Geometry.Down[2] }, CubeEdgeShape.BackwardLeft, Geometry.Right),
			1 => (Array.Empty<VertexPositionNormalTexture>(), Geometry.Backward, new[] { Geometry.Up[0], Geometry.Up[2], Geometry.Up[3] },
				new[] { Geometry.Down[0], Geometry.Down[2], Geometry.Down[3] }, Geometry.Left, CubeEdgeShape.ForwardRight),
			0 => (Array.Empty<VertexPositionNormalTexture>(), Geometry.Backward, new[] { Geometry.Up[1], Geometry.Up[2], Geometry.Up[3] },
				new[] { Geometry.Down[0], Geometry.Down[1], Geometry.Down[3] }, CubeEdgeShape.ForwardLeft, Geometry.Right),
			_ => throw new InvalidOperationException()
		};
	}
}
