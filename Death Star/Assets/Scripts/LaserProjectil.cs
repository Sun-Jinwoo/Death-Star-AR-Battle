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
        // Solo reacciona a CriticalPoints
        CriticalPoint cp = other.GetComponent<CriticalPoint>();
        if (cp != null)
        {
            cp.TakeHit();
            ObjectPool.Instance.Return(gameObject);
        }
        // Si golpea el body (inmune), simplemente se destruye visualmente
        else if (other.gameObject.layer == LayerMask.NameToLayer("DeathStarBody"))
        {
            ObjectPool.Instance.Return(gameObject);
        }
    }
}