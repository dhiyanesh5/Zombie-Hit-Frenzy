using UnityEngine;

public class CarCollisionHandler : MonoBehaviour
{
    [Header("Hit Settings")]
    [SerializeField] private float minSpeedToHit = 2f;
    [SerializeField] private float hitForce = 8f;

    private Rigidbody carRigidbody;

    private void Awake()
    {
        carRigidbody = GetComponentInParent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (carRigidbody == null) return;

        float speed = carRigidbody.linearVelocity.magnitude;
        if (speed < minSpeedToHit) return;

        IHittable hittable = collision.gameObject.GetComponent<IHittable>();
        if (hittable == null) return;

        Vector3 hitDirection = collision.contacts[0].normal * -1f;
        hitDirection.y = 0.3f; // slight upward force for visual feel

        hittable.OnHit(hitDirection, hitForce * speed);
    }
}