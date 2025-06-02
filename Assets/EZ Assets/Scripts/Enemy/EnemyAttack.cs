using System.Collections;
using UnityEngine;

/// <summary>
/// Manages enemy combat behaviors including attack detection and animation triggering
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
public class EnemyAttack : MonoBehaviour
{
    #region Private Fields
    private Transform player;
    private EnemyMovement enemyMovement;
    private IAnimationManager animationManager;
    private EnemyHitReceiver hitReceiver;

    // References to detect player
    private int playerLayerMask;

    // Internal state tracking
    private bool playerInAttackRange = false;
    private float lastAttackTime = 0f;
    private int currentAttackIndex = 0;
    #endregion

    #region Inspector Fields
    [Header("Detection Settings")]
    [Tooltip("Distance at which enemy stops approaching player")]
    public float stoppingDistance = 2.5f;

    [Tooltip("Height of the detection capsule")]
    public float capsuleHeight = 2f;

    [Tooltip("Radius of the detection capsule")]
    public float capsuleRadius = 1f;

    [Header("Attack Settings")]
    [Tooltip("Cooldown between attacks in seconds")]
    public float attackCooldown = 1.5f;

    [Tooltip("Delay before hit detection occurs for regular attacks")]
    public float attackDelay = 0.2f;

    [Tooltip("Randomize which attack to perform")]
    public bool randomizeAttacks = true;

    [Tooltip("Chance to perform a hold attack (0-1)")]
    [Range(0, 1)]
    public float holdAttackChance = 0.3f;

    [Tooltip("Chance to perform a kick instead of punch (0-1)")]
    [Range(0, 1)]
    public float kickChance = 0.4f;
    #endregion

    #region Initialization
    void Start()
    {
        // Find required components
        enemyMovement = GetComponent<EnemyMovement>();
        animationManager = GetComponent<IAnimationManager>();
        hitReceiver = GetComponent<EnemyHitReceiver>();

        // Find player and setup layer mask
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerLayerMask = LayerMask.GetMask("Player");

        // Verify required components
        if (enemyMovement == null)
            Debug.LogError("EnemyMovement component not found on " + gameObject.name);

        if (animationManager == null)
            Debug.LogError("IAnimationManager not found on " + gameObject.name);

        if (hitReceiver == null)
            Debug.LogError("HitReceiver not found on " + gameObject.name);

        if (player == null)
            Debug.LogError("Player not found in scene");
    }
    #endregion

    #region Update Logic
    void Update()
    {
        if (player == null) return;

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Update NavMeshAgent stopping distance dynamically
        UpdateStoppingDistance(distanceToPlayer);

        // Check if player is in attack range using capsule overlap
        playerInAttackRange = IsPlayerInAttackRange();
        
        if (playerInAttackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
            
        }
    }
    #endregion

    #region Movement Control
    /// <summary>
    /// Updates the stopping distance in NavMeshAgent based on player proximity
    /// </summary>
    private void UpdateStoppingDistance(float distanceToPlayer)
    {
        if (distanceToPlayer <= stoppingDistance)
        {
            // Get NavMeshAgent from EnemyMovement and update stopping distance
            var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = true;

                // Rotate towards player
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0; // Keep rotation on horizontal plane

                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }
        }
        else
        {
            // Continue pursuing player
            var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = false;
            }
        }
    }
    #endregion

    #region Combat Detection
    /// <summary>
    /// Checks if player is within attack range using a capsule overlap
    /// </summary>
    private bool IsPlayerInAttackRange()
    {
        // Calculate capsule endpoints
        Vector3 center = transform.position + transform.forward * capsuleRadius;
        Vector3 point1 = center + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 point2 = center - Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);

        // Perform capsule overlap check
        Collider[] hits = Physics.OverlapCapsule(point1, point2, capsuleRadius, playerLayerMask);

        // Debug visualization
        if (hits.Length > 0)
        {
            Debug.DrawLine(point1, point2, Color.red);
        }
        else
        {
            Debug.DrawLine(point1, point2, Color.green);
        }

        return hits.Length > 0;
    }
    #endregion

    #region Attack Logic
    /// <summary>
    /// Perform an attack based on configured probabilities
    /// </summary>
    private void PerformAttack()
    {
        // Determine attack type based on probabilities or sequence
        AttackType attackType = DetermineAttackType();

        // Execute the selected attack
        ExecuteAttack(attackType);
    }

    /// <summary>
    /// Determines which attack to perform based on configuration
    /// </summary>
    private AttackType DetermineAttackType()
    {
        if (randomizeAttacks)
        {
            // Determine if this will be a hold attack
            bool isHoldAttack = Random.value < holdAttackChance;

            // Determine if this will be a kick or punch
            bool isKick = Random.value < kickChance;

            if (isHoldAttack)
            {
                return isKick ? AttackType.HoldKick : AttackType.HoldPunch;
            }
            else
            {
                return isKick ? AttackType.Kick : AttackType.Punch;
            }
        }
        else
        {
            // Cycle through attacks in sequence
            currentAttackIndex = (currentAttackIndex + 1) % 4;
            return (AttackType)currentAttackIndex;
        }
    }

    /// <summary>
    /// Executes the selected attack type and performs hit detection
    /// </summary>
    private void ExecuteAttack(AttackType attackType)
    {
        HitType hitType;
        float delay;

        // Execute attack based on type
        switch (attackType)
        {
            case AttackType.Punch:
                hitType = animationManager.PlayHeadPunch();
                delay = attackDelay;
                break;

            case AttackType.HoldPunch:
                hitType = animationManager.PlayHoldPunch();
                delay = attackDelay * 1.5f; // Longer delay for hold attacks
                break;

            case AttackType.Kick:
                hitType = animationManager.PlayKick();
                delay = attackDelay * 1.2f; // Slightly longer delay for kicks
                break;

            case AttackType.HoldKick:
                hitType = animationManager.PlayHoldKick();
                delay = attackDelay * 1.7f; // Longest delay for hold kicks
                break;

            default:
                hitType = HitType.HeadPunch;
                delay = attackDelay;
                break;
        }

        // Perform hit detection after appropriate delay
        hitReceiver.ReceiveHit(hitType, delay, capsuleHeight, capsuleRadius, playerLayerMask);
    }
    #endregion

    #region Helper Types
    /// <summary>
    /// Enum representing different attack types the enemy can perform
    /// </summary>
    private enum AttackType
    {
        Punch = 0,
        HoldPunch = 1,
        Kick = 2,
        HoldKick = 3
    }
    #endregion

    #region Gizmo Visualization
    /// <summary>
    /// Draw attack range gizmo in editor
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Draw stopping distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // Draw attack range
        Vector3 center = transform.position + transform.forward * capsuleRadius;
        Vector3 point1 = center + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 point2 = center - Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);

        Gizmos.color = playerInAttackRange ? Color.red : Color.green;
        DrawWireCapsule(point1, point2, capsuleRadius);
    }

    /// <summary>
    /// Helper method to draw wire capsule gizmo
    /// </summary>
    private void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
    {
        // Draw end spheres
        Gizmos.DrawWireSphere(p1, radius);
        Gizmos.DrawWireSphere(p2, radius);

        // Calculate direction vectors
        Vector3 up = (p2 - p1).normalized;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized;

        // Draw connecting lines
        Gizmos.DrawLine(p1 + right * radius, p2 + right * radius);
        Gizmos.DrawLine(p1 - right * radius, p2 - right * radius);

        forward = Vector3.Cross(up, right).normalized;
        Gizmos.DrawLine(p1 + forward * radius, p2 + forward * radius);
        Gizmos.DrawLine(p1 - forward * radius, p2 - forward * radius);
    }
    #endregion
}
