namespace Prototype.Systems.Maps;

using FileFormats;
using Stride.Core.Mathematics;
using Stride.Graphics;

public static class TileSetBuilder
{
	public static int TilesPerDirection(Map map)
	{
		return MathUtil.NextPowerOfTwo((int)Math.Ceiling(Math.Sqrt(map.TileSet.Tiles.Count)));
	}

	public static Texture Build(GraphicsContext graphicsContext, Map map)
	{
		var textures = map.TileSet.Tiles.Select(
				texture =>
				{
					using var stream = File.OpenRead($"Assets/Textures/{texture}_diffuse.png");

					return Image.Load(stream);
				}
			)
			.ToArray();

		var tilesPerDirection = TileSetBuilder.TilesPerDirection(map);

		var tileWidth = MathUtil.NextPowerOfTwo(textures.Max(texture => texture.Description.Width));
		var tileHeight = MathUtil.NextPowerOfTwo(textures.Max(texture => texture.Description.Width));

		var textureWidth = tileWidth * tilesPerDirection;
		var textureHeight = tileHeight * tilesPerDirection;

		var tileSet = new uint[textureWidth * textureHeight];

		for (var i = 0; i < map.TileSet.Tiles.Count; i++)
		{
			var tileX = i % tilesPerDirection;
			var tileY = i / tilesPerDirection;

			var tile = textures[i].PixelBuffer[0].GetPixels<uint>();

			for (var y = 0; y < tileHeight; y++)
				Array.Copy(tile, y * tileWidth, tileSet, (tileY * tileHeight + y) * textureWidth + tileX * tileWidth, tileWidth);
		}

		var texture = Texture.New2D(graphicsContext.CommandList.GraphicsDevice, textureWidth, textureHeight, PixelFormat.B8G8R8A8_UNorm_SRgb);
		texture.SetData(graphicsContext.CommandList, tileSet);

		return texture;
	}
}
