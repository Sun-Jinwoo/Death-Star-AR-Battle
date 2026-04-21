using UnityEngine;

/// <summary>
/// Proyectil del jugador (y aliados si reutilizas el prefab).
/// Se autodevuelve al pool tras X segundos o al impactar.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float damage = 50f;
    [SerializeField] private float lifetime = 3f;

    private ObjectPool _sourcePool;
    private float _timer;

    public void Init(ObjectPool pool)
    {
        _sourcePool = pool;
        _timer = lifetime;
    }

    private void Update()
    {
        // Avanza hacia adelante
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));

        _timer -= Time.deltaTime;
        if (_timer <= 0f) ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        // ¿Impactó un punto crítico?
        if (other.TryGetComponent<CriticalPoint>(out var cp))
        {
            cp.ReceiveHit(damage);
            ReturnToPool();
            return;
        }

        // Impactos en la carcasa (DeathStarShell) o enemigos: simplemente destruye
        if (other.gameObject.layer != LayerMask.NameToLayer("Projectile"))
            ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_sourcePool != null)
            _sourcePool.Release(gameObject);
        else
            gameObject.SetActive(false);
    }
}