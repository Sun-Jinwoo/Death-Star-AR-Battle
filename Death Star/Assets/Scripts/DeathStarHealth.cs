using System;
using System.Collections;
using UnityEngine;

public class DeathStarHealth : MonoBehaviour
{
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private GameObject explosionPrefab; // prefab de explosión grande, opcional

    public float CurrentHP { get; private set; }
    public float HPPercent => CurrentHP / maxHP;

    public event Action OnVictory;
    public event Action<float> OnHPChanged;

    private bool victoryFired = false;

    void Awake() => CurrentHP = maxHP;

    public void TakeDamage(float amount)
    {
        if (victoryFired) return;

        // Inmune si hay enemigos vivos
        if (EnemySpawner.Instance != null && EnemySpawner.Instance.HasActiveEnemies)
        {
            return;
        }

        CurrentHP = Mathf.Max(0f, CurrentHP - amount);
        OnHPChanged?.Invoke(HPPercent);

        if (HPPercent <= 0.1f && !victoryFired)
        {
            victoryFired = true;
            OnVictory?.Invoke();
            StartCoroutine(DestructionSequence());
        }
    }

    IEnumerator DestructionSequence()
    {
        float elapsed = 0f;
        float duration = 2f;
        Vector3 originalScale = transform.localScale;

        // Shake progresivo de la Death Star
        while (elapsed < duration)
        {
            float shake = Mathf.Lerp(0f, 0.15f, elapsed / duration);
            transform.localPosition += UnityEngine.Random.insideUnitSphere * shake * Time.deltaTime * 20f;

            // Escala decrece al final
            float scaleMult = elapsed > duration * 0.7f
                ? Mathf.Lerp(1f, 0f, (elapsed - duration * 0.7f) / (duration * 0.3f))
                : 1f;

            transform.localScale = originalScale * scaleMult;

            AudioManager.Instance?.Play("Star_Death");

            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}