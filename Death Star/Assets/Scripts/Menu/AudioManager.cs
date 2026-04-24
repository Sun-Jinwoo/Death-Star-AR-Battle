using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instancia;

    [Header("Audio Sources")]
    public AudioSource efectosSource;
    public AudioSource musicaSource;

    [Header("Clips")]
    public AudioClip click;
    public AudioClip hover;
    public AudioClip musicaFondo;

    void Awake()
    {
        // Singleton (solo uno en todo el juego)
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicaFondo != null)
        {
            musicaSource.clip = musicaFondo;
            musicaSource.loop = true;
            musicaSource.Play();
        }
    }

    // 🔊 Sonido de botón
    public void PlayClick()
    {
        efectosSource.PlayOneShot(click);
    }

    // 🔊 Hover (opcional)
    public void PlayHover()
    {
        efectosSource.PlayOneShot(hover);
    }

    // 🔊 Reproducir cualquier sonido
    public void PlaySound(AudioClip clip)
    {
        efectosSource.PlayOneShot(clip);
    }
}