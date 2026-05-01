using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class NativeTextureSender : MonoBehaviour
{
    [DllImport("GfxPluginMyNativePlugin")]
    private static extern void SetSourceTexture(IntPtr nativeTex);

    [SerializeField] private RenderTexture source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (source == null) {
            source = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
            source.name = "NativeSourceRT";
            source.Create();
        }
        SetSourceTexture(source.GetNativeTexturePtr());
    }
}
