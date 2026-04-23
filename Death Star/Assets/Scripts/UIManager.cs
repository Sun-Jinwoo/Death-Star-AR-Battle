using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject crosshair;

    [Header("Pantallas de resultado")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private TextMeshProUGUI winStatsText;
    [SerializeField] private TextMeshProUGUI loseStatsText;

    [Header("Referencias")]
    [SerializeField] private DeathStarHealth deathStarHealth;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float totalTime = 60f;

    private float timeLeft;
    private bool running = false;
    private int hits = 0;

    void Start()
    {
        timeLeft = totalTime;

        // Suscribirse a eventos
        deathStarHealth.OnHPChanged += HandleHPChanged;
        deathStarHealth.OnVictory += () => hits++; // cuenta impactos via eventos opcionales
        gameManager.OnStateChanged += HandleStateChanged;

        // Estado inicial
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        UpdateHPDisplay(1f);
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!running) return;

        timeLeft -= Time.deltaTime;
        UpdateTimerDisplay();

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            running = false;
            gameManager.TriggerDefeat();
        }
    }

    // --- Handlers ---

    void HandleHPChanged(float percent)
    {
        hits++;
        UpdateHPDisplay(percent);
    }

    void HandleStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Playing:
                running = true;
                crosshair.SetActive(true);
                break;

            case GameManager.GameState.Won:
                running = false;
                crosshair.SetActive(false);
                winStatsText.text = $"Tiempo restante: {FormatTime(timeLeft)}\nImpactos: {hits}";
                winScreen.SetActive(true);
                break;

            case GameManager.GameState.Lost:
                running = false;
                crosshair.SetActive(false);
                loseStatsText.text = $"HP restante: {deathStarHealth.HPPercent * 100:F0}%\nImpactos: {hits}";
                loseScreen.SetActive(true);
                break;
        }
    }

    // --- Helpers ---

    void UpdateHPDisplay(float percent)
    {
        if (hpSlider != null)
            hpSlider.value = percent;

        if (hpText != null)
            hpText.text = $"{percent * 100:F0}%";
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        timerText.text = FormatTime(timeLeft);

        // Cambia color cuando quedan menos de 15 segundos
        timerText.color = timeLeft <= 15f ? Color.red : Color.white;
    }

    string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        return $"{m:00}:{s:00}";
    }

    void OnDestroy()
    {
        if (deathStarHealth != null) deathStarHealth.OnHPChanged -= HandleHPChanged;
        if (gameManager != null) gameManager.OnStateChanged -= HandleStateChanged;
    }
}