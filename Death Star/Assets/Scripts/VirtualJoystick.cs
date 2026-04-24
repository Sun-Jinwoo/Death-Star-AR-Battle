using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField] private float maxRadius = 50f;

    public Vector2 Direction { get; private set; }

    private RectTransform bgRect;
    private Vector2 centerPoint;

    void Awake()
    {
        bgRect = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData e)
    {
        centerPoint = e.position;
        UpdateHandle(e.position);
    }

    public void OnDrag(PointerEventData e)
    {
        UpdateHandle(e.position);
    }

    public void OnPointerUp(PointerEventData e)
    {
        Direction = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    void UpdateHandle(Vector2 inputPos)
    {
        Vector2 delta = inputPos - centerPoint;
        Vector2 clamped = Vector2.ClampMagnitude(delta, maxRadius);
        handle.anchoredPosition = clamped;
        Direction = clamped / maxRadius; // normalizado -1 a 1
    }
}