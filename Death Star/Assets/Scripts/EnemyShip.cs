using System.Collections;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [SerializeField] private float orbitSpeed = 30f;
    [SerializeField] private float orbitRadius = 2f;
    [SerializeField] private float fireRate = 2.5f;
    [SerializeField] private float fireAngleThreshold = 30f;

    private Transform target;
    private float orbitAngle;
    private Camera mainCam;

    void Start()
    {
        // Orbita alrededor de la Death Star
        target = FindObjectOfType<DeathStarHealth>()?.transform;
        mainCam = Camera.main;

        // Ángulo inicial aleatorio para que no salgan todos en el mismo punto
        orbitAngle = Random.Range(0f, 360f);

        StartCoroutine(FireRoutine());
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;
        if (target == null) return;

        // Orbita alrededor de la Death Star
        orbitAngle += orbitSpeed * Time.deltaTime;

        float rad = orbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * orbitRadius,
            Mathf.Sin(rad * 0.5f) * orbitRadius * 0.3f,
            Mathf.Sin(rad) * orbitRadius
        );

        transform.position = target.position + offset;

        // Mira hacia la cámara (el jugador)
        Vector3 dirToPlayer = mainCam.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(dirToPlayer);
    }

    IEnumerator FireRoutine()
    {
        while (true)
        {
            // Cadencia variable para que no disparen todos al mismo tiempo
            yield return new WaitForSeconds(fireRate + Random.Range(-0.5f, 0.5f));

            if (GameManager.Instance.State != GameManager.GameState.Playing) continue;

            FireAtPlayer();
        }
    }

    void FireAtPlayer()
    {
        if (mainCam == null) return;

        Vector3 dirToPlayer = (mainCam.transform.position - transform.position).normalized;

        // Solo dispara si está orientado hacia el jugador
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > fireAngleThreshold) return;

        EnemyProjectilPool.Instance.Get(transform.position, dirToPlayer);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}