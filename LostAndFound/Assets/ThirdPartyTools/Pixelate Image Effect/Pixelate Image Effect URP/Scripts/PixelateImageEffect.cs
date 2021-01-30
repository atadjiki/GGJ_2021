using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelateImageEffect : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelateImageEffectSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        [Range(1, 50)]
        public int tileSize = 10;

        public Material material = null;
    }

    public PixelateImageEffectSettings settings = new PixelateImageEffectSettings();

    class CustomRenderPass : ScriptableRenderPass
    {   
        string profilerTag;

        private int sourceId_copy;
        private RenderTargetIdentifier sourceRT_copy;
        public PixelateImageEffectSettings settings = null;
        
        private RenderTargetIdentifier source { get; set; }

        public void Setup(RenderTargetIdentifier source) {
            this.source = source;
        }

        public CustomRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = cameraTextureDescriptor.width;
            var height = cameraTextureDescriptor.height;

            sourceId_copy = Shader.PropertyToID("_SourceRT_Copy");
            cmd.GetTemporaryRT(sourceId_copy, width, height, 0, FilterMode.Bilinear, cameraTextureDescriptor.colorFormat);

            sourceRT_copy = new RenderTargetIdentifier(sourceId_copy);
            
            ConfigureTarget(sourceRT_copy);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            cmd.Blit(source, sourceRT_copy);

            cmd.SetGlobalFloat("_TileSize", settings.tileSize);
            cmd.Blit(sourceRT_copy, source, settings.material, 0);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass scriptablePass;

    public override void Create()
    {
        scriptablePass = new CustomRenderPass("PixelateImageEffect");
        scriptablePass.settings = settings;

        scriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        scriptablePass.Setup(src);
        renderer.EnqueuePass(scriptablePass);
    }
}


