namespace Prototype.Systems.Maps.Rendering.Depth;

using Stride.Core.Mathematics;
using Stride.Graphics;

public struct VertexPositionNormalTextureDepth : IEquatable<VertexPositionNormalTextureDepth>, IVertex
{
	public Vector3 Position;
	public Vector3 Normal;
	public Vector2 TextureCoordinate;
	public float DepthOffset;

	public static readonly VertexDeclaration Layout = new(
		VertexElement.Position<Vector3>(),
		VertexElement.Normal<Vector3>(),
		VertexElement.TextureCoordinate<Vector2>(),
		DepthOffsetShader.VertexElement
	);

	public VertexPositionNormalTextureDepth(Vector3 position, Vector3 normal, Vector2 textureCoordinate, float depthOffset)
	{
		this.Position = position;
		this.Normal = normal;
		this.TextureCoordinate = textureCoordinate;
		this.DepthOffset = depthOffset;
	}

	public bool Equals(VertexPositionNormalTextureDepth other)
	{
		return this.Position.Equals(other.Position)
			&& this.Normal.Equals(other.Normal)
			&& this.TextureCoordinate.Equals(other.TextureCoordinate)
			&& this.DepthOffset.Equals(other.DepthOffset);
	}

	public override bool Equals(object? obj)
	{
		return obj is VertexPositionNormalTextureDepth other && this.Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(this.Position, this.Normal, this.TextureCoordinate, this.DepthOffset);
	}

	public VertexDeclaration GetLayout()
	{
		return VertexPositionNormalTextureDepth.Layout;
	}

	public void FlipWinding()
	{
		this.TextureCoordinate.X = 1.0f - this.TextureCoordinate.X;
		this.DepthOffset *= -1;
	}

	public static bool operator ==(VertexPositionNormalTextureDepth left, VertexPositionNormalTextureDepth right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(VertexPositionNormalTextureDepth left, VertexPositionNormalTextureDepth right)
	{
		return !left.Equals(right);
	}

	public override string ToString()
	{
		return $"Position: {this.Position}, Normal: {this.Normal}, TextureCoordinate: {this.TextureCoordinate}, DepthOffset: {this.DepthOffset}";
	}
}
