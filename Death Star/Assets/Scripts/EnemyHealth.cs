using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHP = 30f;
    private float currentHP;

    void OnEnable()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        Debug.Log($"[EnemyHealth] HP: {currentHP} en {gameObject.name}");

        if (currentHP <= 0f)
            Die();
    }

    void Die()
    {
        Debug.Log($"[EnemyHealth] Die() llamado en {gameObject.name}");

        if (ImpactPool.Instance != null)
            ImpactPool.Instance.Get(transform.position);

        CameraShake.Instance?.Shake(0.2f, 0.1f);

        // Desactiva en lugar de destruir
        gameObject.SetActive(false);
    }
}