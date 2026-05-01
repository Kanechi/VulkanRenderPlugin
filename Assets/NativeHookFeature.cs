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

        [DllImport("GfxPluginMyNativePlugin")]
        public static extern void SetColor(float r, float g, float b, float a);
    }

    class NativeHookPass : ScriptableRenderPass {

        // ログが一度だけ出力されるようにするためのもの
        static bool loggedOnce = false;

        public NativeHookPass() {

            // レンダリングのどの段階でこのパスが実行されるかを指定
            //renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            renderPassEvent = RenderPassEvent.AfterRendering;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

            // プレイ中のみ
            if (!Application.isPlaying)
                return;

            // Gameカメラのみ
            if (renderingData.cameraData.cameraType != CameraType.Game)
                return;

            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan)
                return;

            if (!loggedOnce) {
                Debug.Log("NativeHookPass reached.");
                loggedOnce = true;
            }

            var cmd = CommandBufferPool.Get("NativeHookPass");

            float t = Time.time;
            float r = Mathf.Abs(Mathf.Sin(t));
            float g = Mathf.Abs(Mathf.Sin(t + 2.0f));
            float b = Mathf.Abs(Mathf.Sin(t + 4.0f));


            NativeBridge.SetRenderSize(Screen.width, Screen.height);
            NativeBridge.SetColor(r, g, b, 1.0f);

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
