using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifetime = 3f;

    private float timer;

    // OnEnable se llama cada vez que el pool reactiva el objeto
    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        timer += Time.deltaTime;
        if (timer >= lifetime)
            ObjectPool.Instance.Return(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Golpea CriticalPoint
        CriticalPoint cp = other.GetComponent<CriticalPoint>();
        if (cp != null)
        {
            cp.TakeHit();
            ImpactPool.Instance?.Get(transform.position); // ← explosión aquí
            ObjectPool.Instance.Return(gameObject);
            return;
        }

        // Golpea enemigo
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(10f);
            ImpactPool.Instance?.Get(transform.position); // ← explosión aquí
            ObjectPool.Instance.Return(gameObject);
            return;
        }

        // Golpea el cuerpo inmune de la Death Star
        if (other.gameObject.layer == LayerMask.NameToLayer("DeathStarBody"))
        {
            ImpactPool.Instance?.Get(transform.position); // ← explosión aquí también opcional
            ObjectPool.Instance.Return(gameObject);
        }
    }
}