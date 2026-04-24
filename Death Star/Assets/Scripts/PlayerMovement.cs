using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float maxOffset = 1.5f;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;

        float input = joystick.Direction.x;
        if (Mathf.Abs(input) < 0.1f) return;

        Vector3 newPos = transform.position + Vector3.right * input * moveSpeed * Time.deltaTime;

        // Limita el movimiento lateral
        float clampedX = Mathf.Clamp(
            newPos.x,
            originalPosition.x - maxOffset,
            originalPosition.x + maxOffset
        );

        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}