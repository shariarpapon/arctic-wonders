using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Arctic.Rendering
{
    public class TintEffectRendererFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class Settings
        {
            public Material passMaterial;
            public Color color;
            public int passIndex = 0;
            public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public Settings settings = new Settings();
        TintPass m_Pass;

        public override void Create()
        {
            m_Pass = new TintPass(name);
            m_Pass.SetupMembers(settings.passMaterial, settings.color, settings.passIndex);
            m_Pass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.passMaterial == null) return;
            renderer.EnqueuePass(m_Pass);
        }


        internal class TintPass : ScriptableRenderPass
        {
            Material m_Material;
            Color m_Color;
            int m_PassIndex;
            private static MaterialPropertyBlock s_SharedPropertyBlock = new MaterialPropertyBlock();

            public TintPass(string passName)
            {
                profilingSampler = new ProfilingSampler(passName);
            }

            public void SetupMembers(Material material, Color color, int passIndex)
            {
                m_Material = material;
                m_PassIndex = passIndex;
                m_Color = color;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourcesData = frameData.Get<UniversalResourceData>();
                Debug.Assert(resourcesData.cameraColor.IsValid());

                // Add a raster render pass using the same builder pattern as your example
                using (var builder = renderGraph.AddRasterRenderPass<MainPassData>("TintPass", out var passData, profilingSampler))
                {
                    passData.material = m_Material;
                    passData.color = m_Color;
                    passData.passIndex = m_PassIndex;

                    if(passData.material.HasColor("_Color"))
                        passData.material.SetColor("_Color", passData.color);

                    builder.SetRenderAttachment(resourcesData.activeColorTexture, 0, AccessFlags.Write);

                    builder.SetRenderFunc((MainPassData data, RasterGraphContext rgContext) =>
                    {
                        s_SharedPropertyBlock.Clear();
                        rgContext.cmd.DrawProcedural(Matrix4x4.identity, data.material, data.passIndex, MeshTopology.Triangles, 3, 1, s_SharedPropertyBlock);
                    });
                }
            }

            private class MainPassData
            {
                internal Material material;
                internal Color color;
                internal int passIndex;
            }
        }
    }

}
