using System;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject sceneRoot;
    public enum GameState { WaitingPlacement, Playing, Won, Lost }

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

    // Cambia el estado inicial en Start:
    void Start()
    {
        deathStar.OnVictory += () => SetState(GameState.Won);
        SetState(GameState.WaitingPlacement); // ← antes era Playing
    }

    void Update()
    {
        // Solo en el Editor — presiona Space para saltar el placement
#if UNITY_EDITOR
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            State == GameState.WaitingPlacement)
        {
            Debug.Log("[GameManager] Space presionado — saltando placement");
            sceneRoot.SetActive(true);
            SetState(GameState.Playing);
        }
#endif
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

    public void RestartGame()
    {
        // Recarga la escena actual — resetea todo el estado
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0); // ← índice de MainMenu
    }
}