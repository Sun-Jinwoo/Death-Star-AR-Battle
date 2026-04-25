using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SkyboxController : MonoBehaviour
{
    public static SkyboxController Instance { get; private set; }

    [SerializeField] private Material spaceSkybox;
    [SerializeField] private ARCameraBackground arCameraBackground;

    private Material originalSkybox;
    private CameraClearFlags originalClearFlags;

    void Awake()
    {
        Instance = this;
        originalSkybox = RenderSettings.skybox;
        originalClearFlags = Camera.main.clearFlags;
    }

    public void ActivateSpace()
    {
        // 1. Desactiva el feed de la cámara real
        if (arCameraBackground != null)
            arCameraBackground.enabled = false;

        // 2. Activa el skybox espacial
        if (spaceSkybox != null)
            RenderSettings.skybox = spaceSkybox;

        // 3. Cambia el Clear Flags para mostrar el skybox
        Camera.main.clearFlags = CameraClearFlags.Skybox;

        Debug.Log("[SkyboxController] Espacio activado");
    }

    public void DeactivateSpace()
    {
        if (arCameraBackground != null)
            arCameraBackground.enabled = true;

        RenderSettings.skybox = originalSkybox;
        Camera.main.clearFlags = originalClearFlags;
    }
}