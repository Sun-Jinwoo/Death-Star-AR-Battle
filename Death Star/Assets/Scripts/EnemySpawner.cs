using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 3;
    [SerializeField] private float spawnInterval = 5f;

    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        // Suscribirse al evento de victoria para limpiar enemigos
        var ds = FindObjectOfType<DeathStarHealth>();
        if (ds != null) ds.OnVictory += ClearAllEnemies;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (GameManager.Instance.State != GameManager.GameState.Playing) continue;

            // Limpia enemigos destruidos de la lista
            activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);

            if (activeEnemies.Count < maxEnemies)
                SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        var ds = FindObjectOfType<DeathStarHealth>();
        if (ds == null) return;

        // Spawnea cerca de la Death Star
        Vector3 spawnPos = ds.transform.position + Random.insideUnitSphere * 1.5f;
        var enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);
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