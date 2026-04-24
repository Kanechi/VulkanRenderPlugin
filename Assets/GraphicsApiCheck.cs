using UnityEngine;
using UnityEngine.Rendering;

public class GraphicsApiCheck : MonoBehaviour {
    void Start() {
        Debug.Log(SystemInfo.graphicsDeviceType);
    }
}
