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
            return;
        }
    }

    void Start()
    {
        if (musicaFondo != null && !musicaSource.isPlaying)
        {
            musicaSource.clip = musicaFondo;
            musicaSource.loop = true;
            musicaSource.Play();
        }
    }

    // 🔊 CLICK
    public void PlayClick()
    {
        if (instancia != null && click != null)
        {
            instancia.efectosSource.PlayOneShot(click);
        }
    }

    // 🔊 HOVER
    public void PlayHover()
    {
        if (instancia != null && hover != null)
        {
            instancia.efectosSource.PlayOneShot(hover);
        }
    }

    // 🔊 GENERICO
    public void PlaySound(AudioClip clip)
    {
        if (instancia != null && clip != null)
        {
            instancia.efectosSource.PlayOneShot(clip);
        }
    }
}