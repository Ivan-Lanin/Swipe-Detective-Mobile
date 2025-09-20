using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    // Camera movement boundaries
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private const float _touchCameraSpeed = 0.019f;
    private const float _mouseCameraSpeed = 0.015f;

    private void Update()
    {
        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.fingerId == 0)
            {
                HandleTouch(touch);
            }
        }

        // Handle mouse input
        else if (Input.GetMouseButton(0))
        {
            HandleMouse();
        }
    }

    private void HandleTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = touch.deltaPosition;
            Vector3 newPosition = transform.position + new Vector3(-touchDeltaPosition.x * _touchCameraSpeed, 0, -touchDeltaPosition.y * _touchCameraSpeed);
            // Clamp the new position within the bounds
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.z = Mathf.Clamp(newPosition.z, minBounds.y, maxBounds.y);
            transform.localPosition = newPosition;
        }
    }

    private void HandleMouse()
    {
        float mouseMultiplier = 60f;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector3 newPosition = transform.position + new Vector3(-mouseX * _mouseCameraSpeed * mouseMultiplier, 0, -mouseY * _mouseCameraSpeed * mouseMultiplier);
        // Clamp the new position within the bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.z = Mathf.Clamp(newPosition.z, minBounds.y, maxBounds.y);
        transform.localPosition = newPosition;
    }
}
