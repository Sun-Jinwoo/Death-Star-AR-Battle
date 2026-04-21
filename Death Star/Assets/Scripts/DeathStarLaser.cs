using System.Collections;
using UnityEngine;

/// <summary>
/// Activa la animación/VFX del rayo de la Estrella al recibir el evento de derrota.
/// Asigna el LineRenderer o el VFX del rayo en el Inspector.
/// </summary>
public class DeathStarLaser : MonoBehaviour
{
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private Transform tatooinTarget;     // Transform destino (planeta o punto)
    [SerializeField] private float chargeTime = 2f;
    [SerializeField] private float fireTime = 2f;
    [SerializeField] private ParticleSystem chargeVfx;
    [SerializeField] private ParticleSystem explosionVfx;    // en Tatooine

    private void Start()
    {
        // Suscribirse al evento de derrota
        GameManager.Instance.OnDefeat += OnDefeat;
        laserLine.enabled = false;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDefeat -= OnDefeat;
    }

    private void OnDefeat() => StartCoroutine(FireSequence());

    private IEnumerator FireSequence()
    {
        // Fase de carga
        chargeVfx?.Play();
        yield return new WaitForSeconds(chargeTime);

        // Disparo: anima el rayo de laserOrigin a Tatooine
        laserLine.enabled = true;
        laserLine.SetPosition(0, laserOrigin.position);

        float elapsed = 0f;
        while (elapsed < fireTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fireTime;
            // Interpola la punta del rayo hacia el objetivo
            laserLine.SetPosition(1, Vector3.Lerp(laserOrigin.position, tatooinTarget.position, t));
            yield return null;
        }

        explosionVfx?.Play();
        laserLine.enabled = false;
    }
}