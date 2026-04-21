using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool genérico. Crea un GameObject por tipo de prefab.
/// Uso: ObjectPool.GetPool(prefab).Get() / Release(obj)
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 20;

    private readonly Queue<GameObject> _pool = new();

    // ?? Registro estático de pools por prefab ??????????????????
    private static readonly Dictionary<GameObject, ObjectPool> Pools = new();

    public static ObjectPool GetPool(GameObject prefab)
    {
        if (Pools.TryGetValue(prefab, out var pool)) return pool;

        var go = new GameObject($"Pool_{prefab.name}");
        var newPool = go.AddComponent<ObjectPool>();
        newPool.prefab = prefab;
        newPool.initialSize = 20;
        newPool.Initialize();
        Pools[prefab] = newPool;
        return newPool;
    }

    private void Awake() => Initialize();

    private void Initialize()
    {
        for (int i = 0; i < initialSize; i++)
            _pool.Enqueue(CreateInstance());
    }

    private GameObject CreateInstance()
    {
        var obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        var obj = _pool.Count > 0 ? _pool.Dequeue() : CreateInstance();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        _pool.Enqueue(obj);
    }
}