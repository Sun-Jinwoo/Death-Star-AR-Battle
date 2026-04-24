using System.Collections;
using UnityEngine;
using TMPro;

public class PlacementUI : MonoBehaviour
{
    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI instructionSubText;

    [Header("Animación")]
    [SerializeField] private GameObject scannerFrame;
    [SerializeField] private float pulseSpeed = 1.5f;

    private bool planeFound = false;

    void OnEnable()
    {
        SetScanning();
        StartCoroutine(PulseScanner());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    // Llama esto desde ARPlacementManager cuando detecta un plano
    public void OnPlaneFound()
    {
        if (planeFound) return;
        planeFound = true;
        SetFound();
    }

    // Llama esto si el plano se pierde
    public void OnPlaneLost()
    {
        planeFound = false;
        SetScanning();
    }

    void SetScanning()
    {
        statusText.text = "BUSCANDO SUPERFICIE...";
        statusText.color = new Color(0.18f, 0.80f, 0.44f);
        instructionText.text = "Apunta a una superficie plana";
        instructionSubText.text = "Mueve lentamente sobre una mesa o el piso";
    }

    void SetFound()
    {
        statusText.text = "SUPERFICIE DETECTADA";
        statusText.color = new Color(0.18f, 0.80f, 0.44f);
        instructionText.text = "Toca para colocar la batalla";
        instructionSubText.text = "La Estrella de la Muerte aparecerá\nen la superficie detectada";
    }

    IEnumerator PulseScanner()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float alpha = Mathf.Lerp(0.3f, 1f, t);

            if (scannerFrame != null)
            {
                var img = scannerFrame.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                {
                    var c = img.color;
                    c.a = alpha;
                    img.color = c;
                }
            }
            yield return null;
        }
    }
}