using System;
using UnityEngine;

/// <summary>
/// Gestiona el HP de la Estrella de la Muerte.
/// Asignar en el GameObject raíz de la DeathStar.
/// </summary>
public class DeathStarHealth : MonoBehaviour
{
    [SerializeField] private float maxHp = 1000f;

    public event Action<float> OnHpChanged;   // 0-1 normalizado
    public event Action OnDestroyed;

    public float CurrentHp { get; private set; }
    public float HpNormalized => CurrentHp / maxHp;
    public bool IsDestroyed { get; private set; }

    private void Awake() => CurrentHp = maxHp;

    /// <summary>Llamado por CriticalPoint al recibir impacto.</summary>
    public void TakeDamage(float amount)
    {
        if (IsDestroyed || GameManager.Instance.CurrentState != GameState.Playing) return;

        CurrentHp = Mathf.Max(0f, CurrentHp - amount);
        OnHpChanged?.Invoke(HpNormalized);

        if (HpNormalized < 0.1f)
        {
            IsDestroyed = true;
            OnDestroyed?.Invoke();
            GameManager.Instance.TriggerVictory();
        }
    }
}