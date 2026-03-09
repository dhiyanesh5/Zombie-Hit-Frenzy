using UnityEngine;

// Single Responsibility: ONLY translates SwipeInputHandler output into Prometeo method calls
// Dependency Inversion: depends on SwipeInputHandler abstraction, not touch API directly
[RequireComponent(typeof(PrometeoCarController))]
[RequireComponent(typeof(SwipeInputHandler))]
public class CarInputBridge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SwipeInputHandler swipeInput;

    [Header("Arcade Feel")]
    [SerializeField] private float autoThrottleStrength = 0.3f;
    // ^ car always moves slightly forward even without vertical swipe (arcade feel)

    private PrometeoCarController car;
    private bool wasDecelerating = false;

    private void Awake()
    {
        car = GetComponent<PrometeoCarController>();
        swipeInput = GetComponent<SwipeInputHandler>();

        // Disable Prometeo's own input handling — we take over completely
        car.useTouchControls = false;
    }

    private void Update()
    {
        if (swipeInput == null || car == null) return;

        HandleThrottle();
        HandleSteering();
    }

    private void HandleThrottle()
    {
        float vertical = swipeInput.VerticalInput;

        if (swipeInput.IsTouching)
        {
            CancelInvoke("DecelerateCarProxy");
            wasDecelerating = false;

            if (vertical >= 0f)
            {
                // Forward — always go forward while touching,
                // autoThrottle ensures car moves even on pure horizontal swipe
                car.GoForward();
            }
            else
            {
                // Dragging down = reverse
                car.GoReverse();
            }
        }
        else
        {
            // Finger lifted — decelerate
            car.ThrottleOff();
            if (!wasDecelerating)
            {
                InvokeRepeating("DecelerateCarProxy", 0f, 0.1f);
                wasDecelerating = true;
            }
        }
    }

    private void HandleSteering()
    {
        float horizontal = swipeInput.HorizontalInput;

        if (Mathf.Abs(horizontal) > 0.01f)
        {
            if (horizontal < 0f)
                car.TurnLeft();
            else
                car.TurnRight();
        }
        else
        {
            // No horizontal input — wheels return to center
            car.ResetSteeringAngle();
        }
    }

    // Proxy because InvokeRepeating can't call methods on other components directly
    private void DecelerateCarProxy()
    {
        car.DecelerateCar();
    }
}
