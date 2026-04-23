using UnityEngine;

public class CriticalPoint : MonoBehaviour
{
    [SerializeField] private float damagePerHit = 10f;

    private DeathStarHealth deathStarHealth;

    void Awake()
    {
        // Sube en la jerarquía a buscar DeathStarHealth en el padre
        deathStarHealth = GetComponentInParent<DeathStarHealth>();

        if (deathStarHealth == null)
            Debug.LogError($"[CriticalPoint] {name} no encontró DeathStarHealth en su padre.");
    }

    public void TakeHit()
    {
        deathStarHealth?.TakeDamage(damagePerHit);

        // Flash visual temporal para feedback (opcional aquí)
        Debug.Log($"[CriticalPoint] Impacto en {name}!");
    }
}