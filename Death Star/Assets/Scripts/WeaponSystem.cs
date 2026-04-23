using UnityEngine;
using UnityEngine.InputSystem; // ← nuevo namespace

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float fireRate = 0.3f;

    private float nextFireTime;
    private Camera mainCam;

    void Awake()
    {
        mainCam = GetComponent<Camera>() ?? Camera.main;
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;

        // New Input System: Mouse.current en lugar de Input.GetMouseButton
        bool firing = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (firing && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        // New Input System: Mouse.current.position.ReadValue()
        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        Ray ray = mainCam.ScreenPointToRay(mouseScreen);

        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, 500f, targetLayers)
            ? hit.point
            : ray.origin + ray.direction * 100f;

        Vector3 direction = (targetPoint - firePoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        ObjectPool.Instance.Get(firePoint.position, rotation);
    }
}