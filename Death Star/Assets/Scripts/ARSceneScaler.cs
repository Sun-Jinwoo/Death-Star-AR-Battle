using UnityEngine;
using UnityEngine.InputSystem;

public class ARSceneScaler : MonoBehaviour
{
    [SerializeField] private GameObject sceneRoot;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 3f;
    [SerializeField] private float scaleSpeed = 0.01f;

    private float previousPinchDistance = 0f;

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;
        if (Touchscreen.current == null) return;

        var touch0 = Touchscreen.current.touches[0];
        var touch1 = Touchscreen.current.touches[1];

        // Necesita exactamente 2 dedos
        bool twoFingers = touch0.press.isPressed && touch1.press.isPressed;
        if (!twoFingers)
        {
            previousPinchDistance = 0f;
            return;
        }

        float currentDistance = Vector2.Distance(
            touch0.position.ReadValue(),
            touch1.position.ReadValue()
        );

        if (previousPinchDistance == 0f)
        {
            previousPinchDistance = currentDistance;
            return;
        }

        float delta = currentDistance - previousPinchDistance;
        float currentScale = sceneRoot.transform.localScale.x;
        float newScale = Mathf.Clamp(currentScale + delta * scaleSpeed, minScale, maxScale);

        sceneRoot.transform.localScale = Vector3.one * newScale;
        previousPinchDistance = currentDistance;
    }
}