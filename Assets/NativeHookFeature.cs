using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using System.Runtime.InteropServices;


public class NativeHookFeature : ScriptableRendererFeature
{
    static class NativeBridge 
    {
        [DllImport("GfxPluginMyNativePlugin")]
        public static extern IntPtr GetRenderEventFunc();

        [DllImport("GfxPluginMyNativePlugin")]
        public static extern void SetRenderSize(int width, int height);
    }

    class NativeHookPass : ScriptableRenderPass {

        // ログが一度だけ出力されるようにするためのもの
        static bool loggedOnce = false;

        public NativeHookPass() {

            // レンダリングのどの段階でこのパスが実行されるかを指定
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan)
                return;

            if (!loggedOnce) {
                Debug.Log("NativeHookPass reached.");
                loggedOnce = true;
            }

            var cmd = CommandBufferPool.Get("NativeHookPass");
            NativeBridge.SetRenderSize(Screen.width, Screen.height);
            cmd.IssuePluginEvent(NativeBridge.GetRenderEventFunc(), 1);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    NativeHookPass pass;

    public override void Create() {
        pass = new NativeHookPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(pass);
    }
}
