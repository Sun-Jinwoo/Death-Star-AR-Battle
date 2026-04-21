using System.Collections;
using UnityEngine;

/// <summary>
/// IA simple de nave aliada: orbita la Estrella de la Muerte y dispara
/// a un punto crítico aleatorio a intervalos.
/// </summary>
public class AllyShipAI : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float orbitRadius = 2f;
    [SerializeField] private float orbitSpeed = 40f;  // grados/segundo
    [SerializeField] private float minFireDelay = 1.5f;
    [SerializeField] private float maxFireDelay = 3.5f;

    private Transform _orbitCenter;
    private CriticalPoint[] _targets;
    private ObjectPool _pool;
    private float _orbitAngle;

    public void Init(Transform orbitCenter, CriticalPoint[] targets)
    {
        _orbitCenter = orbitCenter;
        _targets = targets;
        _pool = ObjectPool.GetPool(projectilePrefab);
        _orbitAngle = Random.Range(0f, 360f);
        StartCoroutine(FireRoutine());
    }

    private void Update()
    {
        if (_orbitCenter == null) return;
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        // Orbita simple en el plano horizontal alrededor de la Estrella
        _orbitAngle = (_orbitAngle + orbitSpeed * Time.deltaTime) % 360f;
        float rad = _orbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0.2f, Mathf.Sin(rad)) * orbitRadius;

        transform.position = _orbitCenter.position + offset;
        transform.LookAt(_orbitCenter.position);
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minFireDelay, maxFireDelay));

            if (GameManager.Instance.CurrentState != GameState.Playing) yield break;
            if (_targets == null || _targets.Length == 0) continue;

            // Elige un punto crítico aleatorio
            var target = _targets[Random.Range(0, _targets.Length)];
            Vector3 dir = (target.transform.position - gunPoint.position).normalized;

            var go = _pool.Get(gunPoint.position, Quaternion.LookRotation(dir));
            if (go.TryGetComponent<Projectile>(out var p)) p.Init(_pool);
        }
    }
}