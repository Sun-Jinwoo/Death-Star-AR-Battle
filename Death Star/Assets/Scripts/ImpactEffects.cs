using System.Collections;
using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        ps.Play();
        StartCoroutine(ReturnWhenDone());
    }

    IEnumerator ReturnWhenDone()
    {
        // Espera a que terminen todas las partículas
        yield return new WaitUntil(() => !ps.IsAlive());
        ImpactPool.Instance.Return(gameObject);
    }
}