using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Playing, Won, Lost }

    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Playing;

    public event Action<GameState> OnStateChanged;

    [SerializeField] private DeathStarHealth deathStar;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        deathStar.OnVictory += () => SetState(GameState.Won);
        SetState(GameState.Playing);
    }

    public void SetState(GameState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(newState);
        Debug.Log($"[GameManager] *** Estado: {newState} ***");
    }

    // El temporizador llamará esto cuando se acabe el tiempo
    public void TriggerDefeat() => SetState(GameState.Lost);

    void OnDestroy()
    {
        // Evita memory leaks al salir del play mode
        if (deathStar != null)
            deathStar.OnVictory -= () => SetState(GameState.Won);
    }
}