using System.Collections;
using Ilumisoft.HealthSystem;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyAttack : MonoBehaviour, IEnemyAttack
{
    #region Private Fields
    private Transform target;
    private IEnemyMovement enemyMovement;
    private IAnimationManager animationManager;
    private EnemyHitReceiver hitReceiver;
    private PlayerHitReceiver hitReceiver2;
    private NavMeshAgent agent;

    private int targetLayerMask;
    private bool targetInAttackRange = false;
    public float lastAttackTime = 0f;
    private int currentAttackIndex = 0;
    [Tooltip("Đặt true nếu là đồng minh (đối tượng tấn công là kẻ thù), false nếu là kẻ thù (đối tượng tấn công là người chơi)")]
    public bool isAlly = false;
    #endregion

    #region Inspector Fields
    [Header("Cài đặt chiến đấu")]
    public float damePunch;
    public float dameHoldPunch;
    public float dameKick;
    public float dameHoldKick;

    [Header("Cài đặt phát hiện")]
    [Tooltip("Khoảng cách dừng lại khi tiếp cận mục tiêu")]
    public float stoppingDistance = 0.2f;

    [Tooltip("Chiều cao của vùng phát hiện")]
    public float capsuleHeight = 2f;

    [Tooltip("Bán kính của vùng phát hiện")]
    public float capsuleRadius = 1f;

    [Header("Cài đặt tấn công")]
    [Tooltip("Thời gian chờ giữa các đòn đánh (giây)")]
    public float attackCooldown = 1.5f;

    [Tooltip("Độ trễ trước khi phát hiện trúng đòn đánh")]
    public float attackDelay = 0.2f;

    [Tooltip("Ngẫu nhiên loại đòn đánh")]
    public bool randomizeAttacks = true;

    [Tooltip("Tỷ lệ thực hiện đòn tấn công giữ (0-1)")]
    [Range(0, 1)]
    public float holdAttackChance = 0.3f;

    [Tooltip("Tỷ lệ thực hiện đá thay vì đấm (0-1)")]
    [Range(0, 1)]
    public float kickChance = 0.4f;

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
        enemyMovement = GetComponent<EnemyMovement>();
        animationManager = GetComponent<IAnimationManager>();
        hitReceiver = GetComponent<EnemyHitReceiver>();
        hitReceiver2 = GetComponent<PlayerHitReceiver>();
        agent = GetComponent<NavMeshAgent>();
        targetLayerMask = isAlly ? LayerMask.GetMask("Enemy") : (LayerMask.GetMask("Player") | LayerMask.GetMask("Ally"));

        if (enemyMovement == null)
            Debug.LogError("EnemyMovement not found on " + gameObject.name);
        if (animationManager == null)
            Debug.LogError("IAnimationManager not found on " + gameObject.name);
        if (hitReceiver == null)
            Debug.LogError("EnemyHitReceiver not found on " + gameObject.name);
        if (hitReceiver2 == null)
            Debug.LogError("PlayerHitReceiver not found on " + gameObject.name);
        if (agent == null)
            Debug.LogError("NavMeshAgent not found on " + gameObject.name);

        Debug.Log($"{gameObject.name} - Initialized with targetLayerMask: {(isAlly ? "Enemy" : "Player, Ally")}");
    }
    #endregion

    #region Update Logic
    private bool wasTargetInAttackRange = false;
    public void Attack()
    {
        if (agent == null || !agent.enabled) return;
        target = enemyMovement.FindNearestTarget();
        if (target == null)
        {
            Debug.Log($"{gameObject.name} - No target found for attack.");
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        UpdateStoppingDistance(distanceToTarget);

        targetInAttackRange = IsPlayerInAttackRange();

        if (targetInAttackRange && !wasTargetInAttackRange)
        {
            lastAttackTime = Time.time;
            Debug.Log($"{(isAlly ? "Ally" : "Enemy")} on {gameObject.name} - Target {target.name} entered attack range at: {lastAttackTime}");
        }

        wasTargetInAttackRange = targetInAttackRange;

        if (targetInAttackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            agent.isStopped = true;
            Debug.Log($"{(isAlly ? "Ally" : "Enemy")} on {gameObject.name} - Performing attack on {target.name} at: {lastAttackTime}");
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }
    #endregion

    #region Movement Control
    public void UpdateStoppingDistance(float distanceToTarget)
    {
        if (agent == null || !agent.enabled) return;

        if (distanceToTarget <= stoppingDistance)
        {
            agent.isStopped = true;
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }
    #endregion

    #region Combat Detection
    public bool IsPlayerInAttackRange()
    {
        Vector3 center = transform.position + transform.forward * capsuleRadius;
        Vector3 point1 = center + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 point2 = center - Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        LayerMask layerMask = LayerMask.GetMask("Player") | LayerMask.GetMask("Ally");
        Collider[] hits = Physics.OverlapCapsule(point1, point2, capsuleRadius, layerMask);

        if (hits.Length > 0)
        {
            Debug.DrawLine(point1, point2, Color.red, 1f);
            foreach (var hit in hits)
            {
                Debug.Log($"{("Enemy")} on {gameObject.name} - Detected target: {hit.gameObject.name} in layer: {LayerMask.LayerToName(hit.gameObject.layer)} , targetlayer : {targetLayerMask}");
            }
            return true;
        }
        else
        {
            Debug.DrawLine(point1, point2, Color.green, 1f);
            return false;
        }
    }

    public bool IsEnemyInAttackRange()
    {
        Vector3 center = transform.position + transform.forward * capsuleRadius;
        Vector3 point1 = center + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 point2 = center - Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        LayerMask layerMask = LayerMask.GetMask("Enemy");
        Collider[] hits = Physics.OverlapCapsule(point1, point2, capsuleRadius, layerMask);

        if (hits.Length > 0)
        {
            Debug.DrawLine(point1, point2, Color.red, 1f);
            foreach (var hit in hits)
            {
                Debug.Log($"{("Ally")} on {gameObject.name} - Detected target: {hit.gameObject.name} in layer: {LayerMask.LayerToName(hit.gameObject.layer)} , targetlayer : {targetLayerMask}");
            }
            return true;
        }
        else
        {
            Debug.DrawLine(point1, point2, Color.green, 1f);
            return false;
        }
    }
    #endregion

    #region Attack Logic
    public void PerformAttack()
    {
        AttackType attackType = DetermineAttackType();
        ExecuteAttack(attackType);
    }

    public AttackType DetermineAttackType()
    {
        if (randomizeAttacks)
        {
            bool isHoldAttack = Random.value < holdAttackChance;
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
            currentAttackIndex = (currentAttackIndex + 1) % 4;
            return (AttackType)currentAttackIndex;
        }
    }

    public void ExecuteAttack(AttackType attackType)
    {
        HitType hitType = PlayAttackAnimation(attackType);
        (float dame, float delay) = GetDamageByHitType(attackType);
        Debug.Log($"{(isAlly ? "Ally" : "Enemy")} on {gameObject.name} - Executing attack: {attackType} with damage: {dame}, delay: {delay}");
        hitReceiver?.ReceiveHit(hitType, delay, capsuleHeight, capsuleRadius, dame, LayerMask.GetMask("Player"));
        hitReceiver2?.ReceiveHit(hitType, delay, capsuleHeight, capsuleRadius, dame, LayerMask.GetMask("Enemy"));
    }

    public enum AttackType
    {
        Punch = 0,
        HoldPunch = 1,
        Kick = 2,
        HoldKick = 3
    }
    #endregion

    #region Animation and Damage
    public HitType PlayAttackAnimation(AttackType attackType)
    {
        if (animationManager == null) return HitType.HeadPunch;
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
                return animationManager.PlayHeadPunch();
        }
    }

    public (float dame, float delay) GetDamageByHitType(AttackType hitType)
    {
        switch (hitType)
        {
            case AttackType.Punch:
                return (damePunch, attackDelay);
            case AttackType.HoldPunch:
                return (dameHoldPunch, attackDelay);
            case AttackType.Kick:
                return (dameKick, attackDelay);
            case AttackType.HoldKick:
                return (dameHoldKick, attackDelay);
            default:
                return (0, 0);
        }
    }
    #endregion

    #region Gizmo Visualization
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        Vector3 center = transform.position + transform.forward * capsuleRadius;
        Vector3 point1 = center + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 point2 = center - Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        Gizmos.color = targetInAttackRange ? Color.red : Color.green;
        DrawWireCapsule(point1, point2, capsuleRadius);
    }

    private void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
    {
        Gizmos.DrawWireSphere(p1, radius);
        Gizmos.DrawWireSphere(p2, radius);
        Vector3 up = (p2 - p1).normalized;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized;
        Gizmos.DrawLine(p1 + right * radius, p2 + right * radius);
        Gizmos.DrawLine(p1 - right * radius, p2 - right * radius);
        forward = Vector3.Cross(up, right).normalized;
        Gizmos.DrawLine(p1 + forward * radius, p2 + forward * radius);
        Gizmos.DrawLine(p1 - forward * radius, p2 - forward * radius);
    }
    #endregion
}