namespace Prototype.Maps.Rendering.Depth;

using JetBrains.Annotations;
using Stride.Core;
using Stride.Shaders;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class StrideForwardShadingDepthOffsetEffect : IShaderMixinBuilder
{
	[ModuleInitializer]
	internal static void __Initialize__()
	{
		ShaderMixinManager.Register(nameof(StrideForwardShadingDepthOffsetEffect), new StrideForwardShadingDepthOffsetEffect());
	}

	void IShaderMixinBuilder.Generate(ShaderMixinSource mixin, ShaderMixinContext context)
	{
		context.Mixin(mixin, "StrideForwardShadingEffect");
		context.Mixin(mixin, new ShaderClassString(nameof(DepthOffsetShader), DepthOffsetShader.Shader));
	}
}
