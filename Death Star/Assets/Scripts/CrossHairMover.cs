using UnityEngine;

public class CrosshairMover : MonoBehaviour
{
    public static CrosshairMover Instance { get; private set; }

    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private float moveSpeed = 400f;

    // Límites de la pantalla para la mira
    private Vector2 minPos;
    private Vector2 maxPos;

    public Vector2 CrosshairScreenPos => crosshair.position;

    void Awake()
    {
        Instance = this;
        // Empieza en el centro
        crosshair.anchoredPosition = Vector2.zero;

        // Límites con margen
        float margin = 60f;
        minPos = new Vector2(-Screen.width / 2f + margin, -Screen.height / 2f + margin);
        maxPos = new Vector2(Screen.width / 2f - margin, Screen.height / 2f - margin);
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;

        Vector2 input = joystick.Direction;
        if (input == Vector2.zero) return;

        Vector2 newPos = crosshair.anchoredPosition + input * moveSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
        newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);
        crosshair.anchoredPosition = newPos;
    }
}