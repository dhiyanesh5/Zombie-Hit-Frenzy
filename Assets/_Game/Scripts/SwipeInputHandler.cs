using UnityEngine;

// Single Responsibility: ONLY reads touch input and outputs normalized values
// Other scripts consume these values — this script knows nothing about the car
public class SwipeInputHandler : MonoBehaviour
{
    [Header("Swipe Sensitivity")]
    [SerializeField] private float horizontalSensitivity = 0.5f; // steer sensitivity
    [SerializeField] private float verticalSensitivity = 0.5f;   // throttle sensitivity
    [SerializeField] private float deadZone = 5f; // min pixels moved to register input

    // Output values — consumed by CarInputBridge
    // Horizontal: -1 = full left, 1 = full right
    // Vertical:    0 = no throttle, 1 = full throttle
    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public bool IsTouching { get; private set; }

    private Vector2 touchStartPos;
    private Vector2 currentDelta;

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        HorizontalInput = 0f;
        VerticalInput = 0f;
        IsTouching = false;

        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = touch.position;
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                IsTouching = true;
                currentDelta = touch.position - touchStartPos;

                if (currentDelta.magnitude < deadZone) break;

                // Use Screen.width to normalize — works on any resolution
                HorizontalInput = Mathf.Clamp(
                    currentDelta.x / Screen.width * horizontalSensitivity * 10f,
                    -1f, 1f
                );

                VerticalInput = Mathf.Clamp(
                    currentDelta.y / Screen.height * verticalSensitivity * 10f,
                    -1f, 1f
                );
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                HorizontalInput = 0f;
                VerticalInput = 0f;
                IsTouching = false;
                touchStartPos = Vector2.zero;
                break;
        }
    }
    // Editor testing with mouse (so we can test in Unity editor without a phone)
#if UNITY_EDITOR
    private bool mouseDragging = false;
    private Vector2 mouseDragStart;

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDragStart = Input.mousePosition;
            mouseDragging = true;
        }

        if (Input.GetMouseButton(0) && mouseDragging)
        {
            IsTouching = true;
            Vector2 delta = (Vector2)Input.mousePosition - mouseDragStart;

            if (delta.magnitude >= deadZone)
            {
                HorizontalInput = Mathf.Clamp(
                    delta.x / Screen.width * horizontalSensitivity * 10f,
                    -1f, 1f
                );
                VerticalInput = Mathf.Clamp(
                    delta.y / Screen.height * verticalSensitivity * 10f,
                    -1f, 1f
                );
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseDragging = false;
            IsTouching = false;
            HorizontalInput = 0f;
            VerticalInput = 0f;
        }
    }
}
#endif