using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 20;

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

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0) CreateNew(); // expande el pool si hace falta

        var obj = pool.Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}