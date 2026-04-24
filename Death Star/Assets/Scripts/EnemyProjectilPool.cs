using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectilPool : MonoBehaviour
{
    public static EnemyProjectilPool Instance { get; private set; }

    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 15;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < initialSize; i++)
            CreateNew();
    }

    void CreateNew()
    {
        var obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public GameObject Get(Vector3 position, Vector3 direction)
    {
        if (pool.Count == 0) CreateNew();
        var obj = pool.Dequeue();
        obj.transform.position = position;
        obj.SetActive(true);
        obj.GetComponent<EnemyProjectil>().SetDirection(direction);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}