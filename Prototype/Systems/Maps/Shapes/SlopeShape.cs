namespace Prototype.Systems.Maps.Shapes;

using Stride.Graphics;

public sealed class SlopeShape : Shape
{
	public SlopeShape(int rotation, int steps, int current)
	{
		var step = 1f / steps;
		var low = current * step;
		var high = (current + 1) * step;

		var (tl, tr, bl, br) = rotation switch
		{
			3 => (low, high, low, high),
			2 => (high, low, high, low),
			1 => (high, high, low, low),
			0 => (low, low, high, high),
			_ => throw new InvalidOperationException()
		};

		this.Forward = SlopeShape.Modify(Shape.FullForward, new[] { br, bl, 0, 0 }, true);
		this.Backward = SlopeShape.Modify(Shape.FullBackward, new[] { tl, tr, 0, 0 }, true);
		this.Up = SlopeShape.Modify(Shape.FullUp, new[] { bl, br, tr, tl }, false);
		this.Down = Shape.FullDown;
		this.Left = SlopeShape.Modify(Shape.FullLeft, new[] { bl, tl, 0, 0 }, true);
		this.Right = SlopeShape.Modify(Shape.FullRight, new[] { tr, br, 0, 0 }, true);
	}

	private static VertexPositionNormalTexture[] Modify(VertexPositionNormalTexture[] vertices, IReadOnlyList<float> heights, bool modifyUv)
	{
		return heights.Distinct().Count() switch
		{
			1 => Array.Empty<VertexPositionNormalTexture>(),
			_ => new[] { 0, 1, 2, 3 }.Select(
					i => new VertexPositionNormalTexture(
						new(vertices[i].Position.X, heights[i], vertices[i].Position.Z),
						vertices[i].Normal,
						new(vertices[i].TextureCoordinate.X, modifyUv ? heights[i] : vertices[i].TextureCoordinate.Y)
					)
				)
				.Distinct()
				.ToArray()
		};
	}
}
