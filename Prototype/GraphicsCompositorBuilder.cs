namespace Prototype;

using Stride.Core.Mathematics;
using Stride.Rendering;
using Stride.Rendering.Background;
using Stride.Rendering.Compositing;
using Stride.Rendering.Images;
using Stride.Rendering.LightProbes;
using Stride.Rendering.Lights;
using Stride.Rendering.Materials;
using Stride.Rendering.Shadows;
using Stride.Rendering.Sprites;

public static class GraphicsCompositorBuilder
{
	public static GraphicsCompositor Create()
	{
		var opaqueRenderStage = new RenderStage("Opaque", "Main") { SortMode = new StateChangeSortMode() };
		var transparentRenderStage = new RenderStage("Transparent", "Main") { SortMode = new BackToFrontSortMode() };
		var shadowCasterRenderStage = new RenderStage("ShadowMapCaster", "ShadowMapCaster") { SortMode = new FrontToBackSortMode() };
		var shadowCasterCubeMapRenderStage = new RenderStage("ShadowMapCasterCubeMap", "ShadowMapCasterCubeMap") { SortMode = new FrontToBackSortMode() };

		var shadowCasterParaboloidRenderStage =
			new RenderStage("ShadowMapCasterParaboloid", "ShadowMapCasterParaboloid") { SortMode = new FrontToBackSortMode() };

		var singleView = new ForwardRenderer
		{
			Clear = { Color = Color.Black },
			OpaqueRenderStage = opaqueRenderStage,
			TransparentRenderStage = transparentRenderStage,
			ShadowMapRenderStages = { shadowCasterRenderStage, shadowCasterParaboloidRenderStage, shadowCasterCubeMapRenderStage },
			PostEffects = new PostProcessingEffects
			{
				DepthOfField = { Enabled = false },
				ColorTransforms = { Transforms = { new ToneMap() } } // TODO make sure ToneMap does not make the night look like day!
			}
		};

		var cameraSlot = new SceneCameraSlot { Name = "Main" };

		return new()
		{
			Cameras = { cameraSlot },
			RenderStages =
			{
				opaqueRenderStage, transparentRenderStage, shadowCasterRenderStage, shadowCasterParaboloidRenderStage, shadowCasterCubeMapRenderStage
			},
			RenderFeatures =
			{
				new MeshRenderFeature
				{
					RenderFeatures =
					{
						new TransformRenderFeature(),
						new SkinningRenderFeature(),
						new MaterialRenderFeature(),
						new ShadowCasterRenderFeature(),
						new ForwardLightingRenderFeature
						{
							LightRenderers =
							{
								new LightAmbientRenderer(),
								new LightSkyboxRenderer(),
								new LightDirectionalGroupRenderer(),
								new LightPointGroupRenderer(),
								new LightSpotGroupRenderer(),
								new LightClusteredPointSpotGroupRenderer(),
								new LightProbeRenderer()
							},
							ShadowMapRenderer = new ShadowMapRenderer
							{
								Renderers =
								{
									new LightDirectionalShadowMapRenderer { ShadowCasterRenderStage = shadowCasterRenderStage },
									new LightSpotShadowMapRenderer { ShadowCasterRenderStage = shadowCasterRenderStage },
									new LightPointShadowMapRendererParaboloid
									{
										ShadowCasterRenderStage = shadowCasterParaboloidRenderStage
									},
									new LightPointShadowMapRendererCubeMap
									{
										ShadowCasterRenderStage = shadowCasterCubeMapRenderStage
									}
								}
							}
						},
						new InstancingRenderFeature()
					},
					RenderStageSelectors =
					{
						new MeshTransparentRenderStageSelector
						{
							EffectName = "StrideForwardShadingDepthOffsetEffect",
							OpaqueRenderStage = opaqueRenderStage,
							TransparentRenderStage = transparentRenderStage
						},
						new ShadowMapRenderStageSelector
						{
							EffectName = "StrideForwardShadingEffect.ShadowMapCaster", ShadowMapRenderStage = shadowCasterRenderStage
						},
						new ShadowMapRenderStageSelector
						{
							EffectName = "StrideForwardShadingEffect.ShadowMapCasterParaboloid",
							ShadowMapRenderStage = shadowCasterParaboloidRenderStage
						},
						new ShadowMapRenderStageSelector
						{
							EffectName = "StrideForwardShadingEffect.ShadowMapCasterCubeMap",
							ShadowMapRenderStage = shadowCasterCubeMapRenderStage
						}
					},
					PipelineProcessors =
					{
						new MeshPipelineProcessor { TransparentRenderStage = transparentRenderStage },
						new ShadowMeshPipelineProcessor { ShadowMapRenderStage = shadowCasterRenderStage },
						new ShadowMeshPipelineProcessor { ShadowMapRenderStage = shadowCasterParaboloidRenderStage, DepthClipping = true },
						new ShadowMeshPipelineProcessor { ShadowMapRenderStage = shadowCasterCubeMapRenderStage, DepthClipping = true }
					}
				},
				new SpriteRenderFeature
				{
					RenderStageSelectors =
					{
						new SpriteTransparentRenderStageSelector
						{
							EffectName = "Test", OpaqueRenderStage = opaqueRenderStage, TransparentRenderStage = transparentRenderStage
						}
					}
				},
				new BackgroundRenderFeature
				{
					RenderStageSelectors = { new SimpleGroupToRenderStageSelector { RenderStage = opaqueRenderStage, EffectName = "Test" } }
				}
			},
			Game = new SceneCameraRenderer { Child = singleView, Camera = cameraSlot },
			Editor = singleView,
			SingleView = singleView
		};
	}
}
