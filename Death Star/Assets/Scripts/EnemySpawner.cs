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

        // Spawnea los 3 enemigos una sola vez
        for (int i = 0; i < maxEnemies; i++)
            SpawnEnemy();

        OnEnemySpawned?.Invoke();

        // Espera hasta que todos mueran
        yield return new WaitUntil(() => !HasActiveEnemies);

        Debug.Log("[EnemySpawner] Todos los enemigos eliminados — estrella vulnerable");
        OnAllEnemiesDefeated?.Invoke();
    }

    void SpawnEnemy()
    {
        var ds = UnityEngine.Object.FindFirstObjectByType<DeathStarHealth>();
        if (ds == null) return;

        Vector3 spawnPos = ds.transform.position + (UnityEngine.Random.insideUnitSphere * 0.3f);
        var enemy = Instantiate(enemyPrefab, spawnPos, enemyPrefab.transform.rotation);
        enemy.transform.SetParent(transform);
        activeEnemies.Add(enemy);

        Debug.Log($"[EnemySpawner] Enemigo {activeEnemies.Count}/{maxEnemies} spawneado");
    }

    public void ClearAllEnemies()
    {
        foreach (var e in activeEnemies)
            if (e != null) Destroy(e);
        activeEnemies.Clear();
    }
}