using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }

    // duration: cuánto dura | magnitude: qué tan fuerte
    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines(); // evita shakes solapados
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Shake decrece suavemente al final
            float strength = Mathf.Lerp(magnitude, 0f, elapsed / duration);
            transform.localPosition = originalPos + Random.insideUnitSphere * strength;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}