using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class ARPlacementManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private GameObject sceneRoot;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlacementUI placementUI;

    private bool scenePlaced = false;
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void OnEnable()
    {
        // Suscribe al evento de detección de planos para notificar al UI
        arPlaneManager.trackablesChanged.AddListener(OnPlanesChanged);
    }

    void OnDisable()
    {
        arPlaneManager.trackablesChanged.RemoveListener(OnPlanesChanged);
    }

    void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (scenePlaced) return;

        // Notifica al PlacementUI cuando detecta el primer plano
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

            sceneRoot.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            sceneRoot.SetActive(true);

            scenePlaced = true;

            StopPlaneDetection();

            placementUI?.gameObject.SetActive(false);
            gameManager.SetState(GameManager.GameState.Playing);
        }
    }

    void StopPlaneDetection()
    {
        // 1. Desactiva el manager — no detecta nuevos planos
        arPlaneManager.enabled = false;

        // 2. Oculta Y desactiva cada plano ya detectado completamente
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        // 3. Limpia la lista de hits para liberar memoria
        hits.Clear();
    }
}