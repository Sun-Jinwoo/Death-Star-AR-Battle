using System.Collections.Generic;
using UnityEngine;

public class ImpactPool : MonoBehaviour
{
    public static ImpactPool Instance { get; private set; }

    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private int initialSize = 10;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < initialSize; i++)
            CreateNew();
    }

    void CreateNew()
    {
        var obj = Instantiate(impactPrefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public GameObject Get(Vector3 position)
    {
        if (pool.Count == 0) CreateNew();
        var obj = pool.Dequeue();
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}