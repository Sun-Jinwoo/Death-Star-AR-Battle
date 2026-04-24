using UnityEngine;

public class SimpleExplosion : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        // Crea el Particle System por código
        ps = gameObject.AddComponent<ParticleSystem>();

        // Detiene el sistema para configurarlo
        ps.Stop();

        // Main module
        var main = ps.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = 0.4f;
        main.startSpeed = 2f;
        main.startSize = 0.1f;
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.5f, 0f),   // naranja
            new Color(1f, 0.2f, 0f)    // rojo
        );
        main.maxParticles = 30;
        main.stopAction = ParticleSystemStopAction.Destroy;

        // Emission — todas las partículas de golpe
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 30)
        });

        // Shape — esfera
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.05f;

        // Color over lifetime — desvanece
        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(1f, 0.5f, 0f), 0f),
                new GradientColorKey(new Color(1f, 0.2f, 0f), 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)   // ← desvanece al final
            }
        );
        col.color = new ParticleSystem.MinMaxGradient(gradient);

        // Size over lifetime — decrece
        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(1f, 0f);
        sol.size = new ParticleSystem.MinMaxCurve(1f, curve);
    }

    public void Play()
    {
        ps.Play();
    }
}