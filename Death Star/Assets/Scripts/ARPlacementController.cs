using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Detecta un tap sobre un plano AR y coloca el SceneRoot una sola vez.
/// Una vez colocado, oculta la detección/visualización de planos y deshabilita
/// el reticle de colocación. Emite eventos para que GameManager avance de estado.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class ARPlacementController : MonoBehaviour
{
    [Header("Referencias AR")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private Camera arCamera;

    [Header("Escena a colocar")]
    [Tooltip("Prefab raíz que contiene Death Star, naves, etc. Se instancia una sola vez.")]
    [SerializeField] private GameObject sceneRootPrefab;

    [Tooltip("Indicador visual opcional que se mueve sobre el plano detectado antes del tap.")]
    [SerializeField] private GameObject placementReticle;

    [Header("Opciones")]
    [Tooltip("Si está activo, oculta los planos detectados tras colocar la escena.")]
    [SerializeField] private bool hidePlanesAfterPlacement = true;

    [Tooltip("Si está activo, rota el SceneRoot en Y para que mire a la cámara al colocarse.")]
    [SerializeField] private bool faceCameraOnPlacement = true;

    [Header("Eventos")]
    public UnityEvent OnScenePlaced;

    // Estado interno
    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private GameObject spawnedSceneRoot;
    private bool isPlaced;

    // Exposed read-only por si otros sistemas lo necesitan
    public GameObject SpawnedSceneRoot => spawnedSceneRoot;
    public bool IsPlaced => isPlaced;

    private void Reset()
    {
        // Auto-asignación cómoda cuando agregas el componente
        raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
    }

    private void Update()
    {
        if (isPlaced) return;

        // 1) Actualiza reticle mirando al centro de la pantalla
        UpdateReticle();

        // 2) Detecta tap válido (solo dedo, ignora toques sobre UI)
        if (!TryGetTouchPosition(out Vector2 screenPos)) return;

        // 3) Raycast sobre planos detectados
        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            TrackableId hitPlaneId = hits[0].trackableId;
            PlaceScene(hitPose, hitPlaneId);
        }
    }

    /// <summary>
    /// Mueve el reticle al primer plano bajo el centro de la pantalla.
    /// </summary>
    private void UpdateReticle()
    {
        if (placementReticle == null) return;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            if (!placementReticle.activeSelf) placementReticle.SetActive(true);
            placementReticle.transform.SetPositionAndRotation(hits[0].pose.position, hits[0].pose.rotation);
        }
        else
        {
            if (placementReticle.activeSelf) placementReticle.SetActive(false);
        }
    }

    /// <summary>
    /// Devuelve la posición del tap si hubo uno este frame. Ignora toques sobre UI.
    /// </summary>
    private bool TryGetTouchPosition(out Vector2 screenPos)
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began && !IsPointerOverUI(t.fingerId))
            {
                screenPos = t.position;
                return true;
            }
        }
#if UNITY_EDITOR
        // Atajo para probar en editor con click izquierdo
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI(-1))
        {
            screenPos = Input.mousePosition;
            return true;
        }
#endif
        screenPos = default;
        return false;
    }

    private bool IsPointerOverUI(int fingerId)
    {
        var eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem == null) return false;
        return fingerId < 0
            ? eventSystem.IsPointerOverGameObject()
            : eventSystem.IsPointerOverGameObject(fingerId);
    }

    private void PlaceScene(Pose pose, TrackableId planeId)
    {
        if (sceneRootPrefab == null)
        {
            Debug.LogError("[ARPlacementController] sceneRootPrefab no está asignado.");
            return;
        }

        // Rotar opcionalmente hacia la cámara en Y (mantener horizontal)
        Quaternion rotation = pose.rotation;
        if (faceCameraOnPlacement && arCamera != null)
        {
            Vector3 toCamera = arCamera.transform.position - pose.position;
            toCamera.y = 0f;
            if (toCamera.sqrMagnitude > 0.0001f)
                rotation = Quaternion.LookRotation(toCamera.normalized, Vector3.up);
        }

        // Intentamos usar ARAnchor para estabilidad, con fallback a instancia libre
        spawnedSceneRoot = SpawnAnchoredOrFree(pose.position, rotation, planeId);

        isPlaced = true;

        if (placementReticle != null) placementReticle.SetActive(false);
        if (hidePlanesAfterPlacement) HideAllPlanes();

        OnScenePlaced?.Invoke();
    }

    /// <summary>
    /// Crea el SceneRoot anclado a un ARAnchor si es posible. Si no, lo instancia libre.
    /// </summary>
    private GameObject SpawnAnchoredOrFree(Vector3 position, Quaternion rotation, TrackableId planeId)
    {
        if (anchorManager != null && planeManager != null)
        {
            ARPlane plane = planeManager.GetPlane(planeId);
            if (plane != null)
            {
                var anchor = anchorManager.AttachAnchor(plane, new Pose(position, rotation));
                if (anchor != null)
                {
                    GameObject root = Instantiate(sceneRootPrefab, anchor.transform);
                    root.transform.localPosition = Vector3.zero;
                    root.transform.localRotation = Quaternion.identity;
                    return root;
                }
            }
        }

        // Fallback: sin anchor
        return Instantiate(sceneRootPrefab, position, rotation);
    }

    private void HideAllPlanes()
    {
        if (planeManager == null) return;

        foreach (var plane in planeManager.trackables)
            plane.gameObject.SetActive(false);

        // Detener la detección para liberar recursos
        planeManager.enabled = false;
    }
}