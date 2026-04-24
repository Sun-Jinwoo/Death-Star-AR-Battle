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

            // Posiciona el SceneRoot
            sceneRoot.transform.SetPositionAndRotation(
                hitPose.position,
                hitPose.rotation
            );
            sceneRoot.SetActive(true);

            // Ancla el objeto al mundo real
            AnchorScene(hits[0]);

            scenePlaced = true;
            StopPlaneDetection();

            placementUI?.gameObject.SetActive(false);
            gameManager.SetState(GameManager.GameState.Playing);
        }
    }

    void AnchorScene(ARRaycastHit hit)
    {
        // Crea un ARAnchor en el punto tocado
        var anchor = arAnchorManager.AttachAnchor(
            hit.trackable as ARPlane,
            hit.pose
        );

        if (anchor != null)
        {
            // El SceneRoot se vuelve hijo del ancla
            // así queda clavado en el mundo real
            sceneRoot.transform.SetParent(anchor.transform);
            Debug.Log("[AR] Escena anclada correctamente");
        }
        else
        {
            Debug.LogWarning("[AR] No se pudo crear el ancla — objeto puede flotar");
        }
    }

    void StopPlaneDetection()
    {
        arPlaneManager.enabled = false;
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(false);
        hits.Clear();
    }
}