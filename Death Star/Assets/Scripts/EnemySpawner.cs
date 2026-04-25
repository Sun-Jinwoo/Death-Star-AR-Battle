using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 3;
    [SerializeField] private float initialDelay = 3f;

    public event Action OnEnemySpawned;
    public event Action OnAllEnemiesDefeated; // ← nuevo evento

    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    public bool HasActiveEnemies
    {
        get
        {
            activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);
            return activeEnemies.Count > 0;
        }
    }

    void Awake()
    {
        Instance = this;
        StartCoroutine(SpawnRoutine());
    }

    /*void Start()
    {
        
    }*/

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        if (GameManager.Instance.State != GameManager.GameState.Playing) yield break;

        // Spawea uno por uno con delay entre cada uno
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy(i);
            OnEnemySpawned?.Invoke();
            yield return new WaitForSeconds(2f); // ← espera 2s entre cada spawn
        }

        // Espera hasta que todos mueran
        yield return new WaitUntil(() => !HasActiveEnemies);

        Debug.Log("[EnemySpawner] Todos eliminados — estrella vulnerable");
        OnAllEnemiesDefeated?.Invoke();
    }

    void SpawnEnemy(int index)
    {
        var ds = UnityEngine.Object.FindFirstObjectByType<DeathStarHealth>();
        if (ds == null) return;

        // Ángulo fijo por índice + variación aleatoria
        float baseAngle = (360f / maxEnemies) * index;
        float randomAngle = baseAngle + UnityEngine.Random.Range(-30f, 30f);
        float rad = randomAngle * Mathf.Deg2Rad;

        // Altura aleatoria
        float height = UnityEngine.Random.Range(-2f, 2f);

        // Radio grande — ajusta según tu escena
        float radius = 5f;

        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * radius,
            height,
            Mathf.Sin(rad) * radius
        );

        Vector3 spawnPos = ds.transform.position + offset;
        var enemy = Instantiate(enemyPrefab, spawnPos, enemyPrefab.transform.rotation);
        enemy.transform.SetParent(transform);
        activeEnemies.Add(enemy);

        Debug.Log($"[EnemySpawner] Enemigo {index} — ángulo {randomAngle:F0}° — pos {spawnPos}");
    }
    public void ClearAllEnemies()
    {
        foreach (var e in activeEnemies)
            if (e != null) Destroy(e);
        activeEnemies.Clear();
    }
}