using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHP = 30f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private EnemyHealthBar healthBar; // ← nuevo

    private float currentHP;

    void OnEnable()
    {
        currentHP = maxHP;
        healthBar?.UpdateBar(1f); // ← resetea la barra al spawnear
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(0f, currentHP);

        Debug.Log($"[EnemyHealth] HP: {currentHP} en {gameObject.name}");

        // Actualiza la barra
        healthBar?.UpdateBar(currentHP / maxHP);

        if (currentHP <= 0f)
            Die();
    }

    void Die()
    {
        if (explosionPrefab != null)
        {
            var exp = Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );
            exp.GetComponent<SimpleExplosion>()?.Play();
        }

        CameraShake.Instance?.Shake(0.2f, 0.1f);
        gameObject.SetActive(false);
    }
}