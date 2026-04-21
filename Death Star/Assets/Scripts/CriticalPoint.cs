using UnityEngine;

/// <summary>
/// Punto crítico de la Estrella de la Muerte.
/// Requiere un Collider (puede ser trigger o sólido).
/// El DeathStarHealth debe estar en un padre de este GameObject.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CriticalPoint : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private ParticleSystem hitVfx;          // efecto al impactar

    // Cacheado en Awake para no usar GetComponentInParent en cada impacto
    private DeathStarHealth _deathStarHealth;

    private void Awake()
    {
        _deathStarHealth = GetComponentInParent<DeathStarHealth>();
        if (_deathStarHealth == null)
            Debug.LogError($"[CriticalPoint] No se encontró DeathStarHealth en los padres de {name}");
    }

    /// <summary>Llamado por el proyectil al colisionar.</summary>
    public void ReceiveHit(float baseDamage)
    {
        _deathStarHealth?.TakeDamage(baseDamage * damageMultiplier);

        if (hitVfx != null)
        {
            hitVfx.transform.position = transform.position;
            hitVfx.Play();
        }
    }
}