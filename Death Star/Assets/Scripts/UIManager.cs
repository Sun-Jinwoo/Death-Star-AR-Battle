using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Actualiza toda la UI en respuesta a eventos del GameManager y DeathStarHealth.
/// Asignar en el Inspector: hpSlider, timerText, crosshair, victoryPanel, defeatPanel.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image crosshair;

    [Header("Pantallas finales")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [Header("Referencias")]
    [SerializeField] private DeathStarHealth deathStarHealth;

    private void Start()
    {
        GameManager.Instance.OnTimerUpdated += UpdateTimer;
        GameManager.Instance.OnVictory += ShowVictory;
        GameManager.Instance.OnDefeat += ShowDefeat;
        deathStarHealth.OnHpChanged += UpdateHp;

        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        hpSlider.value = 1f;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnTimerUpdated -= UpdateTimer;
        GameManager.Instance.OnVictory -= ShowVictory;
        GameManager.Instance.OnDefeat -= ShowDefeat;
        deathStarHealth.OnHpChanged -= UpdateHp;
    }

    private void UpdateTimer(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        timerText.text = $"{m:00}:{s:00}";

        // Parpadeo rojo en los últimos 10 segundos
        timerText.color = seconds <= 10f
            ? Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 3f, 1f))
            : Color.white;
    }

    private void UpdateHp(float normalized) => hpSlider.value = normalized;

    private void ShowVictory() { victoryPanel.SetActive(true); crosshair.enabled = false; }
    private void ShowDefeat() { defeatPanel.SetActive(true); crosshair.enabled = false; }
}