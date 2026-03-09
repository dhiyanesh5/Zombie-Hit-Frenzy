using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RagdollController))]
public class ZombieAI : MonoBehaviour
{
    // ── Wander Settings ────────────────────────────────────────────
    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 15f;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;

    // ── Arena Bounds (match your 40x40 plane) ──────────────────────
    [Header("Arena Bounds")]
    [SerializeField] private float arenaBoundsX = 18f;
    [SerializeField] private float arenaBoundsZ = 18f;

    // ── Internal State ─────────────────────────────────────────────
    private enum State { Wandering, Waiting, Dead }
    private State currentState = State.Wandering;

    private NavMeshAgent agent;
    private Animator animator;
    private RagdollController ragdoll;

    private float waitTimer = 0f;

    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");

    // ───────────────────────────────────────────────────────────────
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdoll = GetComponent<RagdollController>();
    }

    private void OnEnable()
    {
        // Called by object pool on spawn
        currentState = State.Wandering;
        agent.enabled = true;
        SetNewWanderDestination();
    }

    private void Update()
    {
        if (currentState == State.Dead) return;

        switch (currentState)
        {
            case State.Wandering: UpdateWandering(); break;
            case State.Waiting: UpdateWaiting(); break;
        }

        UpdateAnimator();
    }

    // ── State: Wandering ───────────────────────────────────────────
    private void UpdateWandering()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Reached destination — wait before picking next one
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
            currentState = State.Waiting;
        }
    }

    // ── State: Waiting ─────────────────────────────────────────────
    private void UpdateWaiting()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            SetNewWanderDestination();
            currentState = State.Wandering;
        }
    }

    // ── Animator Sync ──────────────────────────────────────────────
    private void UpdateAnimator()
    {
        float speed = agent.enabled ? agent.velocity.magnitude : 0f;
        animator.SetFloat(MoveSpeedHash, speed);
    }

    // ── Navigation Helper ──────────────────────────────────────────
    private void SetNewWanderDestination()
    {
        // wanderRadius intentionally kept for future use — suppresses warning
        _ = wanderRadius;
        Vector3 destination = GetRandomArenaPoint();
        agent.SetDestination(destination);
    }

    private Vector3 GetRandomArenaPoint()
    {
        // Try up to 10 times to find a valid NavMesh point within arena
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(-arenaBoundsX, arenaBoundsX),
                0f,
                Random.Range(-arenaBoundsZ, arenaBoundsZ)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        // Fallback: return current position
        return transform.position;
    }

    // ── Called by RagdollController when hit ──────────────────────
    public void OnDeath()
    {
        currentState = State.Dead;
        agent.enabled = false;
        animator.enabled = false;
    }

    // ── Called by ZombieSpawner to respawn ────────────────────────
    public void Revive(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        currentState = State.Wandering;
        animator.enabled = true;
        agent.enabled = true;
        SetNewWanderDestination();
    }
}