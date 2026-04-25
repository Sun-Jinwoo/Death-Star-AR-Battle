using UnityEngine;
using UnityEngine.InputSystem;

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

    // Llamado por el FireButton
    public void TryFire()
    {
        Debug.Log("[WeaponSystem] TryFire ejecutado");
        if (GameManager.Instance.State != GameManager.GameState.Playing)
        {
            Debug.Log($"[WeaponSystem] Estado incorrecto: {GameManager.Instance.State}");
            return;
        }
        if (Time.time < nextFireTime)
        {
            Debug.Log("[WeaponSystem] En cooldown");
            return;
        }

        Fire();
        nextFireTime = Time.time + fireRate;
    }

    void Fire()
    {
        // Usa la posición de la mira en pantalla
        Vector2 screenPos = CrosshairMover.Instance.CrosshairScreenPos;
        Ray ray = mainCam.ScreenPointToRay(screenPos);

        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, 500f, targetLayers)
            ? hit.point
            : ray.origin + ray.direction * 100f;

        Vector3 direction = (targetPoint - firePoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        ObjectPool.Instance.Get(firePoint.position, rotation);

        AudioManager.Instance?.Play("Player_Fire");
    }
}