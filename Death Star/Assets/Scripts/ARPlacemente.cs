using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ARPlacementManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private ARAnchorManager arAnchorManager;  // ← NUEVO
    [SerializeField] private GameObject sceneRoot;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlacementUI placementUI;

    private bool scenePlaced = false;
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void OnEnable()
    {
        arPlaneManager.trackablesChanged.AddListener(OnPlanesChanged);
    }

    void OnDisable()
    {
        arPlaneManager.trackablesChanged.RemoveListener(OnPlanesChanged);
    }

    void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (scenePlaced) return;
        if (arPlaneManager.trackables.count > 0)
            placementUI?.OnPlaneFound();
    }

    void Update()
    {
        if (scenePlaced) return;
        if (gameManager.State != GameManager.GameState.WaitingPlacement) return;
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;
        if (!touch.press.wasPressedThisFrame) return;

        Vector2 touchPos = touch.position.ReadValue();

        if (arRaycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            sceneRoot.transform.SetPositionAndRotation(
                hitPose.position,
                hitPose.rotation
            );
            sceneRoot.SetActive(true);

            // ← AGREGA ESTA LÍNEA
            SkyboxController.Instance?.ActivateSpace();

            scenePlaced = true;
            StopPlaneDetection();

            placementUI?.gameObject.SetActive(false);
            gameManager.SetState(GameManager.GameState.Playing);
        }
    }

    void AnchorScene(ARRaycastHit hit)
    {
        // Desparentea por si acaso tiene algún padre
        sceneRoot.transform.SetParent(null);

        // Fija posición y rotación en coordenadas del mundo AR
        sceneRoot.transform.position = hit.pose.position;
        sceneRoot.transform.rotation = hit.pose.rotation;

        // Congela el Rigidbody si tiene alguno
        var rb = sceneRoot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        Debug.Log($"[AR] Escena fijada en {hit.pose.position}");
    }

    void StopPlaneDetection()
    {
        arPlaneManager.enabled = false;
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(false);
        hits.Clear();
    }
}