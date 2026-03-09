using UnityEngine;

// Any object that can be hit by the car implements this
public interface IHittable
{
    void OnHit(Vector3 hitDirection, float hitForce);
}