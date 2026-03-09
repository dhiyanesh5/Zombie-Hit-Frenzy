using System.Collections;
using UnityEngine;

public class RagdollController : MonoBehaviour, IHittable
{
    [Header("Ragdoll Settings")]
    [SerializeField] private float ragdollDuration = 3f;
    [SerializeField] private float hitForceMultiplier = 1.5f;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Animator animator;
    private ZombieAI zombieAI;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        zombieAI = GetComponent<ZombieAI>();

        // Get ALL rigidbodies and colliders in children (the ragdoll bones)
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Start with ragdoll OFF — animated state
        SetRagdollActive(false);
    }

    // ── IHittable Implementation ───────────────────────────────────
    public void OnHit(Vector3 hitDirection, float hitForce)
    {
        if (isDead) return;
        isDead = true;

        zombieAI.OnDeath();
        SetRagdollActive(true);
        ApplyHitForce(hitDirection, hitForce * hitForceMultiplier);

        StartCoroutine(RespawnAfterDelay());
    }

    // ── Ragdoll Toggle ─────────────────────────────────────────────
    private void SetRagdollActive(bool active)
    {
        animator.enabled = !active;

        foreach (var rb in ragdollBodies)
        {
            // Skip the ROOT rigidbody (the one on this same GameObject)
            if (rb.transform == transform) continue;
            rb.isKinematic = !active;
        }

        foreach (var col in ragdollColliders)
        {
            // Skip the root CapsuleCollider
            if (col.transform == transform) continue;
            col.enabled = active;
        }
    }

    // ── Hit Physics ────────────────────────────────────────────────
    private void ApplyHitForce(Vector3 direction, float force)
    {
        // Apply force to all ragdoll bones for full-body reaction
        foreach (var rb in ragdollBodies)
        {
            if (rb.transform == transform) continue;
            rb.AddForce(direction.normalized * force, ForceMode.Impulse);
        }
    }

    // ── Respawn Coroutine ──────────────────────────────────────────
    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(ragdollDuration);

        isDead = false;
        SetRagdollActive(false);

        // TODO Phase 5: ZombieSpawner will handle repositioning
        // For now just revive in place
        zombieAI.Revive(transform.position);
    }
}