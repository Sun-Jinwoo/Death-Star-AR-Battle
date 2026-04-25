using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        // Siempre mira hacia la cámara
        if (mainCam != null)
            transform.LookAt(
                transform.position + mainCam.transform.rotation * Vector3.forward,
                mainCam.transform.rotation * Vector3.up
            );
    }

    public void UpdateBar(float percent)
    {
        if (fill != null)
            fill.fillAmount = percent;
    }
}