using UnityEngine;

/// <summary>
/// Sistema de disparo del jugador.
/// Usa un Raycast desde el centro de la cámara para calcular la dirección
/// y luego instancia el proyectil desde GunPoint.
/// Asignar: gunPoint, projectilePrefab, camera (AR Camera).
/// </summary>
public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private Transform gunPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Camera arCamera;
    [SerializeField] private float fireRate = 0.3f;   // disparos por segundo
    [SerializeField] private LayerMask aimLayerMask;          // CriticalPoint + DeathStarShell

    private float _nextFireTime;
    private ObjectPool _pool;

    private void Start()
    {
        _pool = ObjectPool.GetPool(projectilePrefab);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        // Botón de disparo: tap en pantalla (fuera del área de UI) o Space en editor
        bool fireInput = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                      || Input.GetKeyDown(KeyCode.Space);

        if (fireInput && Time.time >= _nextFireTime)
            Fire();
    }

    private void Fire()
    {
        _nextFireTime = Time.time + fireRate;

        // Raycast desde el centro de la cámara para encontrar el punto de impacto
        Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, 200f, aimLayerMask)
            ? hit.point
            : ray.GetPoint(100f);

        // Orienta el proyectil hacia el objetivo
        Vector3 direction = (targetPoint - gunPoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        var projGO = _pool.Get(gunPoint.position, rotation);
        if (projGO.TryGetComponent<Projectile>(out var proj))
            proj.Init(_pool);
    }
}