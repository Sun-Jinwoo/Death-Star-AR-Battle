using UnityEngine;

public class EnemyProjectil : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private float damage = 10f;

    private float timer;
    private Vector3 direction;

    void OnEnable()
    {
        timer = 0f;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        if (timer >= lifetime)
            EnemyProjectilPool.Instance.Return(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Daña al jugador si golpea el SceneRoot o un collider del jugador
        if (other.CompareTag("Player"))
        {
            PlayerHealth.Instance?.TakeDamage(damage);
            EnemyProjectilPool.Instance.Return(gameObject);
        }
    }
}