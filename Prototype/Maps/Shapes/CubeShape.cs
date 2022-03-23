﻿namespace Prototype.Maps.Shapes;

using Stride.Core.Mathematics;
using Stride.Graphics;

public class CubeShape : Shape
{
	public CubeShape(Vector3 min, Vector3 max)
	{
		this.Forward = CubeShape.Modify(Geometry.Forward, min, max, new(1 - max.X, min.Y), new(1 - min.X, max.Y));
		this.Backward = CubeShape.Modify(Geometry.Backward, min, max, new(min.X, min.Y), new(max.X, max.Y));
		this.Up = CubeShape.Modify(Geometry.Up, min, max, new(min.X, min.Z), new(max.X, max.Z));
		this.Down = CubeShape.Modify(Geometry.Down, min, max, new(min.X, min.Z), new(max.X, max.Z));
		this.Left = CubeShape.Modify(Geometry.Left, min, max, new(min.Z, min.Y), new(max.Z, max.Y));
		this.Right = CubeShape.Modify(Geometry.Right, min, max, new(1 - max.Z, min.Y), new(1 - min.Z, max.Y));
	}

	private static VertexPositionNormalTexture[] Modify(
		IEnumerable<VertexPositionNormalTexture> vertices,
		Vector3 positionMin,
		Vector3 positionMax,
		Vector2 textureCoordinateMin,
		Vector2 textureCoordinateMax
	)
	{
		return vertices.Select(
				vertex => new VertexPositionNormalTexture(
					new(
						Math.Clamp(vertex.Position.X, positionMin.X, positionMax.X),
						Math.Clamp(vertex.Position.Y, positionMin.Y, positionMax.Y),
						Math.Clamp(vertex.Position.Z, positionMin.Z, positionMax.Z)
					),
					vertex.Normal,
					new(
						Math.Clamp(vertex.TextureCoordinate.X, textureCoordinateMin.X, textureCoordinateMax.X),
						Math.Clamp(vertex.TextureCoordinate.Y, textureCoordinateMin.Y, textureCoordinateMax.Y)
					)
				)
			)
			.ToArray();
	}
}
