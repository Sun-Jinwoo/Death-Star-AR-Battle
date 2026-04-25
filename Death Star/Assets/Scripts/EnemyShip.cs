using System.Collections;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [SerializeField] private float orbitSpeed = 30f;
    [SerializeField] private float orbitRadius = 2f;
    [SerializeField] private float fireRate = 2.5f;

    private Quaternion prefabRotationOffset;
    private Transform target;
    private float orbitAngle;
    private Camera mainCam;

    void Start()
    {
        prefabRotationOffset = transform.localRotation;
        target = Object.FindFirstObjectByType<DeathStarHealth>()?.transform;

        // Busca la cámara de forma más robusta
        mainCam = Camera.main;

        if (mainCam == null)

        if (target == null)

        orbitAngle = Random.Range(0f, 360f);
        StartCoroutine(FireRoutine());
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;
        if (target == null) return;

        if (mainCam == null)
        {
            mainCam = Camera.main; // intenta encontrarla de nuevo
            return;
        }

        orbitAngle += orbitSpeed * Time.deltaTime;

        float rad = orbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * orbitRadius,
            Mathf.Sin(rad * 0.5f) * orbitRadius * 0.3f,
            Mathf.Sin(rad) * orbitRadius
        );

        transform.position = target.position + offset;

        // Mira hacia el jugador + aplica el offset del prefab
        Vector3 dirToPlayer = mainCam.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer);
        transform.rotation = lookRotation * prefabRotationOffset;
    }

    IEnumerator FireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate + Random.Range(-0.5f, 0.5f));

            if (GameManager.Instance.State != GameManager.GameState.Playing) continue;

            FireAtPlayer();
        }
    }

    void FireAtPlayer()
    {
        if (mainCam == null) return;
        if (EnemyProjectilPool.Instance == null) return;

        Vector3 dirToPlayer = (mainCam.transform.position - transform.position).normalized;

        // ← elimina la verificación de ángulo, siempre dispara
        EnemyProjectilPool.Instance.Get(transform.position, dirToPlayer);

        AudioManager.Instance?.Play("Caza_Canon");
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
}