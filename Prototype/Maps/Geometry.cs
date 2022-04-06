namespace Prototype.Maps;

using FileFormats;
using Rendering.Depth;
using Shapes;
using Stride.Core.Mathematics;
using Stride.Graphics;

public static class Geometry
{
	private const float InnerDepthOffset = 1f / 8192;

	public static readonly VertexPositionNormalTexture[] Right =
	{
		new(new(1, 1, 1), new(1, 0, 0), new(0, 0)),
		new(new(1, 1, 0), new(1, 0, 0), new(1, 0)),
		new(new(1, 0, 0), new(1, 0, 0), new(1, 1)),
		new(new(1, 0, 1), new(1, 0, 0), new(0, 1))
	};

	public static readonly VertexPositionNormalTexture[] Left =
	{
		new(new(0, 1, 0), new(-1, 0, 0), new(0, 0)),
		new(new(0, 1, 1), new(-1, 0, 0), new(1, 0)),
		new(new(0, 0, 1), new(-1, 0, 0), new(1, 1)),
		new(new(0, 0, 0), new(-1, 0, 0), new(0, 1))
	};

	public static readonly VertexPositionNormalTexture[] Up =
	{
		new(new(0, 1, 0), new(0, 1, 0), new(0, 0)),
		new(new(1, 1, 0), new(0, 1, 0), new(1, 0)),
		new(new(1, 1, 1), new(0, 1, 0), new(1, 1)),
		new(new(0, 1, 1), new(0, 1, 0), new(0, 1))
	};

	public static readonly VertexPositionNormalTexture[] Down =
	{
		new(new(1, 0, 1), new(0, -1, 0), new(1, 1)),
		new(new(1, 0, 0), new(0, -1, 0), new(1, 0)),
		new(new(0, 0, 0), new(0, -1, 0), new(0, 0)),
		new(new(0, 0, 1), new(0, -1, 0), new(0, 1))
	};

	public static readonly VertexPositionNormalTexture[] Backward =
	{
		new(new(0, 1, 1), new(0, 0, 1), new(0, 0)),
		new(new(1, 1, 1), new(0, 0, 1), new(1, 0)),
		new(new(1, 0, 1), new(0, 0, 1), new(1, 1)),
		new(new(0, 0, 1), new(0, 0, 1), new(0, 1))
	};

	public static readonly VertexPositionNormalTexture[] Forward =
	{
		new(new(1, 1, 0), new(0, 0, -1), new(0, 0)),
		new(new(0, 1, 0), new(0, 0, -1), new(1, 0)),
		new(new(0, 0, 0), new(0, 0, -1), new(1, 1)),
		new(new(1, 0, 0), new(0, 0, -1), new(0, 1))
	};

	private static readonly Shape[] Shapes = new Shape[66];

	static Geometry()
	{
		for (var i = 0; i < Geometry.Shapes.Length; i++)
		{
			Geometry.Shapes[i] = i switch
			{
				0 => new CubeShape(new(0, 0, 0), new(1, 1, 1)),
				>= 1 and <= 8 => new SlopeShape((i - 1) / 2, 2, (i - 1) % 2),
				>= 9 and <= 40 => new SlopeShape((i - 9) / 8, 8, (i - 1) % 8),
				>= 41 and <= 44 => new SlopeShape((i - 41) / 1, 1, (i - 41) % 1),
				>= 45 and <= 48 => new CubeEdgeShape(i - 45),
				>= 49 and <= 52 => new SlopeEdgeShapeInner(i - 49),
				53 => new CubeShape(new(0, 0, 0), new(.375f, 1, 1)),
				54 => new CubeShape(new(.625f, 0, 0), new(1, 1, 1)),
				55 => new CubeShape(new(0, 0, 0), new(1, 1, .375f)),
				56 => new CubeShape(new(0, 0, .625f), new(1, 1, 1)),
				57 => new CubeShape(new(0, 0, 0), new(.375f, 1, .375f)),
				58 => new CubeShape(new(.625f, 0, 0), new(1, 1, .375f)),
				59 => new CubeShape(new(.625f, 0, .625f), new(1, 1, 1)),
				60 => new CubeShape(new(0, 0, .625f), new(.375f, 1, 1)),
				61 => new CubeShape(new(.375f, 0, .375f), new(.625f, 1, .625f)),
				>= 62 and <= 65 => new SlopeEdgeShapeOuter(i - 62),
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}

	public static (VertexBufferBinding, IndexBufferBinding) BuildBlocks(GraphicsDevice graphicsDevice, Map map)
	{
		var vertices = new List<VertexPositionNormalTextureDepth>();

		for (var z = 0; z < map.Cells.GetLength(2); z++)
		for (var y = 0; y < map.Cells.GetLength(1); y++)
		for (var x = 0; x < map.Cells.GetLength(0); x++)
		{
			var block = map.Cells[x, y, z].Block;

			if (block != null)
				vertices.AddRange(Geometry.ApplyOffset(Geometry.Shapes[block.ShapeType].BuildBlock(map, block), new(x, y, z)));
		}

		var indices = Enumerable.Range(0, vertices.Count).ToArray();

		return (new(Buffer.New(graphicsDevice, vertices.ToArray(), BufferFlags.VertexBuffer), VertexPositionNormalTextureDepth.Layout, vertices.Count),
			new(Buffer.New(graphicsDevice, indices, BufferFlags.IndexBuffer), true, indices.Length));
	}

	private static IEnumerable<VertexPositionNormalTextureDepth> ApplyOffset(IEnumerable<VertexPositionNormalTextureDepth> vertices, Vector3 offset)
	{
		return vertices.Select(
			vertex => new VertexPositionNormalTextureDepth(vertex.Position + offset, vertex.Normal, vertex.TextureCoordinate, vertex.DepthOffset)
		);
	}

	public static (VertexBufferBinding, IndexBufferBinding) BuildLiquids(GraphicsDevice graphicsDevice, Map map)
	{
		var vertices = new List<VertexPositionNormalColor>();

		for (var z = 0; z < map.Cells.GetLength(2); z++)
		for (var y = 0; y < map.Cells.GetLength(1); y++)
		for (var x = 0; x < map.Cells.GetLength(0); x++)
		{
			var liquid = map.Cells[x, y, z].Liquid;

			if (liquid != null)
				vertices.AddRange(Geometry.ApplyOffset(Geometry.Shapes[liquid.ShapeType].BuildLiquid(liquid), new(x, y, z)));
		}

		var indices = Enumerable.Range(0, vertices.Count).ToArray();

		return (new(Buffer.New(graphicsDevice, vertices.ToArray(), BufferFlags.VertexBuffer), VertexPositionNormalColor.Layout, vertices.Count),
			new(Buffer.New(graphicsDevice, indices, BufferFlags.IndexBuffer), true, indices.Length));
	}

	private static IEnumerable<VertexPositionNormalColor> ApplyOffset(IEnumerable<VertexPositionNormalColor> vertices, Vector3 offset)
	{
		return vertices.Select(vertex => new VertexPositionNormalColor(vertex.Position + offset, vertex.Normal, vertex.Color));
	}

	public static IEnumerable<VertexPositionNormalTextureDepth> Build(Map map, IEnumerable<VertexPositionNormalTexture> vertices, Side? side, bool inner)
	{
		if (side == null)
			return Array.Empty<VertexPositionNormalTextureDepth>();

		var tilesPerDirection = TileSetBuilder.TilesPerDirection(map);

		var x = side.Material % tilesPerDirection;
		var y = side.Material / tilesPerDirection;
		var step = 1f / tilesPerDirection;
		var from = new Vector2(x, y) * step;

		var newVertices = vertices.Select(
				vertex =>
				{
					var textureCoordinate = vertex.TextureCoordinate;

					if (side.Flip)
						textureCoordinate.X = 1 - textureCoordinate.X;

					(textureCoordinate.X, textureCoordinate.Y) = side.Rotation switch
					{
						3 => (1 - textureCoordinate.Y, textureCoordinate.X),
						2 => (1 - textureCoordinate.X, 1 - textureCoordinate.Y),
						1 => (textureCoordinate.Y, 1 - textureCoordinate.X),
						_ => (textureCoordinate.X, textureCoordinate.Y)
					};

					return new VertexPositionNormalTextureDepth(
						vertex.Position,
						vertex.Normal * (inner ? -1 : 1),
						textureCoordinate * step + from,
						inner ? Geometry.InnerDepthOffset : 0
					);
				}
			)
			.ToArray();

		if (newVertices.Length == 4)
			newVertices = new[] { newVertices[0], newVertices[1], newVertices[2], newVertices[2], newVertices[3], newVertices[0] };

		if (inner)
			Array.Reverse(newVertices);

		return newVertices;
	}

	public static IEnumerable<VertexPositionNormalColor> Build(IEnumerable<VertexPositionNormalColor> vertices)
	{
		var newVertices = vertices.ToArray();

		if (newVertices.Length == 4)
			newVertices = new[] { newVertices[0], newVertices[1], newVertices[2], newVertices[2], newVertices[3], newVertices[0] };

		return newVertices;
	}
}
