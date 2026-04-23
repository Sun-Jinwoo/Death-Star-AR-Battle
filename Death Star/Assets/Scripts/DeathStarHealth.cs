using System;
using UnityEngine;

public class DeathStarHealth : MonoBehaviour
{
    [SerializeField] private float maxHP = 100f;

    public float CurrentHP { get; private set; }
    public float HPPercent => CurrentHP / maxHP;

    // Otros sistemas se suscriben a estos eventos (UIManager, GameManager)
    public event Action OnVictory;
    public event Action<float> OnHPChanged; // pasa el % de 0 a 1

    private bool victoryFired = false;

    void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        if (victoryFired) return; // evita daño post-victoria

        CurrentHP = Mathf.Max(0f, CurrentHP - amount);
        OnHPChanged?.Invoke(HPPercent);

        Debug.Log($"[DeathStar] HP: {CurrentHP:F1} / {maxHP} ({HPPercent * 100:F0}%)");

        if (HPPercent <= 0.1f && !victoryFired)
        {
            victoryFired = true;
            OnVictory?.Invoke();
        }
    }
}