using UnityEngine;

public class CarCollisionHandler : MonoBehaviour
{
    [Header("Hit Settings")]
    [SerializeField] private float minSpeedToHit = 2f;
    [SerializeField] private float hitForce = 0.5f;

    private Rigidbody carRigidbody;

    private void Awake()
    {
        
        carRigidbody = GetComponent<Rigidbody>();
        if (carRigidbody == null)
            carRigidbody = GetComponentInParent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (carRigidbody == null) return;

        float speed = carRigidbody.linearVelocity.magnitude;
        if (speed < minSpeedToHit) return;

        IHittable hittable = collision.gameObject.GetComponent<IHittable>();
        if (hittable == null)
            hittable = collision.gameObject.GetComponentInParent<IHittable>();

        // Guard — hitting a wall or floor has no IHittable, just ignore it
        if (hittable == null) return;

        Vector3 hitDirection = collision.contacts[0].normal * -1f;
        hitDirection.y = 0.3f;
        hittable.OnHit(hitDirection, hitForce * speed);
    }
}