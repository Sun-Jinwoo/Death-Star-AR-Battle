using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [SerializeField] private float maxHP = 100f;

    public float CurrentHP { get; private set; }
    public float HPPercent => CurrentHP / maxHP;

    public event Action<float> OnHPChanged;
    public event Action OnPlayerDead;

    private bool isDead = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CurrentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        CurrentHP = Mathf.Max(0f, CurrentHP - amount);
        OnHPChanged?.Invoke(HPPercent);
        CameraShake.Instance?.Shake(0.2f, 0.1f);

        if (CurrentHP <= 0f && !isDead)
        {
            isDead = true;
            OnPlayerDead?.Invoke();
            GameManager.Instance.TriggerDefeat();
        }
    }
}