using System.Collections;
using Ilumisoft.HealthSystem;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages enemy combat behaviors including attack detection and animation triggering
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
public class EnemyAttack : MonoBehaviour, IEnemyAttack
{
    #region Private Fields
    private Transform player;
    private IEnemyMovement enemyMovement;
    private IAnimationManager animationManager;
    private EnemyHitReceiver hitReceiver;
    private NavMeshAgent agent;

    // References to detect player
    private int playerLayerMask;

    // Internal state tracking
    private bool playerInAttackRange = false;
    public  float lastAttackTime = 0f;
    private int currentAttackIndex = 0;
    #endregion

    #region Inspector Fields
    [Header("Enemy Combat Settings")]
    public float damePunch;
    public float dameHoldPunch;
    public float dameKick ;
    public float dameHoldKick;


    [Header("Detection Settings")]
    [Tooltip("Distance at which enemy stops approaching player")]
    public float stoppingDistance = 0.2f;

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

    // IEnemyAttack implementation
    public float DamePunch { get => damePunch; set => damePunch = value; }
    public float DameHoldPunch { get => dameHoldPunch; set => dameHoldPunch = value; }
    public float DameKick { get => dameKick; set => dameKick = value; }
    public float DameHoldKick { get => dameHoldKick; set => dameHoldKick = value; }
    public float StoppingDistance { get => stoppingDistance; set => stoppingDistance = value; }
    public float CapsuleHeight { get => capsuleHeight; set => capsuleHeight = value; }
    public float CapsuleRadius { get => capsuleRadius; set => capsuleRadius = value; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public float AttackDelay { get => attackDelay; set => attackDelay = value; }
    public bool RandomizeAttacks { get => randomizeAttacks; set => randomizeAttacks = value; }
    public float HoldAttackChance { get => holdAttackChance; set => holdAttackChance = value; }
    public float KickChance { get => kickChance; set => kickChance = value; }

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
        agent = GetComponent<NavMeshAgent>();

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
    private bool wasPlayerInAttackRange = false;
    public void Attack()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        UpdateStoppingDistance(distanceToPlayer);

        playerInAttackRange = IsPlayerInAttackRange();

        // Chỉ cập nhật lastAttackTime khi player vừa vào vùng tấn công
        if (playerInAttackRange && !wasPlayerInAttackRange)
        {
            lastAttackTime = Time.time;
            Debug.Log("Player just entered attack range, start timing: " + lastAttackTime);
        }

        wasPlayerInAttackRange = playerInAttackRange;

        Debug.Log("playerInAttackRange:" + playerInAttackRange);

        if (playerInAttackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            agent.isStopped = true;
            Debug.Log("Player in attack range, performing attack " + lastAttackTime);
            PerformAttack();
            lastAttackTime = Time.time; // Reset timer sau khi tấn công
        }
    }
    #endregion

    #region Movement Control
    /// <summary>
    /// Updates the stopping distance in NavMeshAgent based on player proximity
    /// </summary>
    public  void UpdateStoppingDistance(float distanceToPlayer)
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
    public bool IsPlayerInAttackRange()
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
    public void PerformAttack()
    {
        // Determine attack type based on probabilities or sequence
        AttackType attackType = DetermineAttackType();

        // Execute the selected attack
        ExecuteAttack(attackType);
    }

    /// <summary>
    /// Determines which attack to perform based on configuration
    /// </summary>
    public AttackType DetermineAttackType()
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
    public void ExecuteAttack(AttackType attackType)
    {
        HitType hitType = PlayAttackAnimation(attackType);
        (float dame, float delay) = GetDamageByHitType(attackType);


        hitReceiver.ReceiveHit(hitType, delay, capsuleHeight, capsuleRadius, dame, playerLayerMask);

    }
    /// <summary>
    /// Enum representing different attack types the enemy can perform
    /// </summary>
    public enum AttackType
    {
        Punch = 0,
        HoldPunch = 1,
        Kick = 2,
        HoldKick = 3
    }
    #endregion


    public HitType PlayAttackAnimation(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.Punch:
                return animationManager.PlayHeadPunch();
            case AttackType.HoldPunch:
                return animationManager.PlayHoldPunch();
            case AttackType.Kick:
                return animationManager.PlayKick();
            case AttackType.HoldKick:
                return animationManager.PlayHoldKick();
            default:
                return HitType.HeadPunch;
        }
    }

    public (float dame, float delay) GetDamageByHitType(AttackType hitType)
    {
        switch (hitType)
        {
            case AttackType.Punch:
                return (damePunch, attackDelay);
            case AttackType.HoldPunch:
                return (damePunch, attackDelay);
            case AttackType.Kick:
                return (damePunch, attackDelay);
            case AttackType.HoldKick:
                return (damePunch, attackDelay);
        }
        return (0,0);
    }






 







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
