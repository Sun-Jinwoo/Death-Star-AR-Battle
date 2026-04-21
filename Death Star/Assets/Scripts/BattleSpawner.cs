using UnityEngine;

/// <summary>
/// Crea las naves aliadas y enemigas al inicio de la batalla.
/// Asignar: allyPrefab, tieFighterPrefab, deathStar (para obtener CriticalPoints),
/// spawnCenter (SceneRoot).
/// </summary>
public class BattleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject allyPrefab;
    [SerializeField] private GameObject tieFighterPrefab;
    [SerializeField] private Transform spawnCenter;
    [SerializeField] private DeathStarHealth deathStar;
    [SerializeField] private int allyCount = 4;
    [SerializeField] private int tieCount = 6;
    [SerializeField] private float spawnRadius = 3f;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Playing) SpawnAll();
    }

    private void SpawnAll()
    {
        var criticals = deathStar.GetComponentsInChildren<CriticalPoint>();

        for (int i = 0; i < allyCount; i++)
        {
            Vector3 pos = spawnCenter.position + Random.insideUnitSphere * spawnRadius;
            var go = Instantiate(allyPrefab, pos, Quaternion.identity);
            if (go.TryGetComponent<AllyShipAI>(out var ai))
                ai.Init(deathStar.transform, criticals);
        }

        for (int i = 0; i < tieCount; i++)
        {
            Vector3 pos = spawnCenter.position + Random.insideUnitSphere * spawnRadius * 1.5f;
            Instantiate(tieFighterPrefab, pos, Quaternion.identity);
            // TIE Fighter AI se puede ampliar: patrulla, persigue al jugador, etc.
        }
    }
}