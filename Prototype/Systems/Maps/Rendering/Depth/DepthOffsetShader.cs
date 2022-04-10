namespace Prototype.Systems.Maps.Rendering.Depth;

using Stride.Graphics;

public static class DepthOffsetShader
{
	private const string AttributeName = "DEPTHOFFSET";

	public const string Shader = $@"
		shader {nameof(DepthOffsetShader)} : ShaderBase
		{{
			stage stream float DepthOffset : {DepthOffsetShader.AttributeName};

			stage override void VSMain()
			{{
				base.VSMain();

				streams.ShadingPosition.z -= streams.DepthOffset;
			}}
		}};
	";

	public static readonly VertexElement VertexElement = new(DepthOffsetShader.AttributeName, 0, VertexElement.ConvertTypeToFormat<float>());
}
