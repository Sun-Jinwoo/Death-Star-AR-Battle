using System.Collections;
using UnityEngine;

public class CriticalPoint : MonoBehaviour
{
    [SerializeField] private float damagePerHit = 10f;
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private float flashDuration = 0.12f;

    private DeathStarHealth deathStarHealth;
    private Renderer rend;
    private Color originalColor;

    void Awake()
    {
        deathStarHealth = GetComponentInParent<DeathStarHealth>();
        rend = GetComponent<Renderer>();

        if (rend != null)
            originalColor = rend.material.color;
    }

    public void TakeHit()
    {
        deathStarHealth?.TakeDamage(damagePerHit);
        CameraShake.Instance?.Shake(0.15f, 0.08f);

        if (rend != null)
            StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        rend.material.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        rend.material.color = originalColor;
    }
}