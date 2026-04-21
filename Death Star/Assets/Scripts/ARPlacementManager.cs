using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Detecta un plano con tap y ancla el SceneRoot una sola vez.
/// Después deshabilita la detección para ahorrar CPU en móvil.
/// </summary>
public class ARPlacementManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private GameObject sceneRoot;        // prefab o GameObject de la batalla
    [SerializeField] private GameObject placementIndicator; // indicador visual (anillo/reticle)

    private bool _placed = false;
    private static readonly List<ARRaycastHit> Hits = new();

    private void Update()
    {
        if (_placed) return;

        // Mueve el indicador al centro de pantalla continuamente
        UpdateIndicator();

        // Tap / click para confirmar posición
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            TryPlace(Input.GetTouch(0).position);

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            TryPlace(Input.mousePosition);
#endif
    }

    private void UpdateIndicator()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        if (arRaycastManager.Raycast(screenCenter, Hits, TrackableType.PlaneWithinPolygon))
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(
                Hits[0].pose.position,
                Hits[0].pose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void TryPlace(Vector2 screenPos)
    {
        if (!arRaycastManager.Raycast(screenPos, Hits, TrackableType.PlaneWithinPolygon))
            return;

        Pose hitPose = Hits[0].pose;

        sceneRoot.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
        sceneRoot.SetActive(true);
        placementIndicator.SetActive(false);

        // Deshabilita detección de planos para liberar GPU/CPU
        arPlaneManager.enabled = false;
        foreach (var plane in arPlaneManager.trackables)
            plane.gameObject.SetActive(false);

        _placed = true;

        GameManager.Instance.StartGame();
    }
}