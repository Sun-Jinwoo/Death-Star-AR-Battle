using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public event Action OnEnemySpawned;
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 3;
    [SerializeField] private float spawnInterval = 5f;

    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    // DeathStar chequea esto antes de recibir daño
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
    }

    void Start()
    {
        var ds = FindObjectOfType<DeathStarHealth>();
        if (ds != null) ds.OnVictory += ClearAllEnemies;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Espera un poco antes del primer spawn
        yield return new WaitForSeconds(3f);

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (GameManager.Instance.State != GameManager.GameState.Playing) continue;

            activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);

            if (activeEnemies.Count < maxEnemies)
                SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        var ds = FindObjectOfType<DeathStarHealth>();
        if (ds == null) return;

        Vector3 spawnPos = ds.transform.position + UnityEngine.Random.insideUnitSphere * 0.3f;
        var enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.transform.SetParent(transform);
        enemy.transform.localScale = Vector3.one * 0.05f;
        activeEnemies.Add(enemy);

        OnEnemySpawned?.Invoke();

        Debug.Log($"[EnemySpawner] Enemigo spawneado — activos: {activeEnemies.Count}");
    }

    void ClearAllEnemies()
    {
        foreach (var e in activeEnemies)
            if (e != null) Destroy(e);
        activeEnemies.Clear();
    }

    void OnDestroy()
    {
        var ds = FindObjectOfType<DeathStarHealth>();
        if (ds != null) ds.OnVictory -= ClearAllEnemies;
    }
}