using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour, IEnemyMovement
{
    private NavMeshAgent agent;
    private Transform target;
    private IAnimationManager animationManager;
    private IEnemyAttack enemyAttack;

    public bool isMoving = false;
    public float speed;
    [Tooltip("Set to true for ally (targets enemies), false for enemy (targets players)")]
    public bool isAlly = false;

    public bool IsMoving => isMoving;
    public float Speed { get => speed; set => speed = value; }

    public float SetAgentSpeed
    {
        get => agent != null ? agent.speed : 0f;
        set { if (agent != null) agent.speed = value; }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animationManager = GetComponent<IAnimationManager>();
        enemyAttack = GetComponent<IEnemyAttack>();

        if (agent == null)
            Debug.LogError($"[{gameObject.name}] NavMeshAgent not found!");
        else
        {
            agent.enabled = true;
            Debug.Log($"[{gameObject.name}] NavMeshAgent initialized with speed: {agent.speed}, enabled: {agent.enabled}");
        }

        if (animationManager == null)
            Debug.LogError($"[{gameObject.name}] IAnimationManager not found!");
        if (enemyAttack == null)
            Debug.LogError($"[{gameObject.name}] IEnemyAttack not found!");
    }

    public void Movement()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            Debug.LogWarning($"[{gameObject.name}] NavMeshAgent invalid or not on NavMesh! Enabled: {(agent != null ? agent.enabled.ToString() : "null")}, IsOnNavMesh: {(agent != null ? agent.isOnNavMesh.ToString() : "null")}");
            return;
        }

        target = FindNearestTarget();
        if (target != null)
        {
            NavMeshPath path = new NavMeshPath();
            bool pathValid = agent.CalculatePath(target.position, path);
            if (pathValid && path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(target.position);
                speed = agent.velocity.magnitude;
                isMoving = speed > 0.2f;
                animationManager?.PlayRun(isMoving);
                Debug.Log($"[{gameObject.name}] Moving to target: {target.name}, position: {target.position}, speed: {speed}, isMoving: {isMoving}");
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] Invalid path to target: {target.name}, position: {target.position}, pathStatus: {path.status}");
                isMoving = false;
                animationManager?.PlayRun(isMoving);
            }
        }
        else
        {
            isMoving = false;
            animationManager?.PlayRun(isMoving);
            Debug.Log($"[{gameObject.name}] No target found, stopping movement.");
        }
    }

    public void Stop()
    {
        if (agent == null || !agent.enabled) return;
        agent.isStopped = true;
        agent.ResetPath();
        isMoving = false;
        animationManager?.PlayRun(isMoving);
        Debug.Log($"[{gameObject.name}] Movement stopped.");
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log($"[{gameObject.name}] Target set to: {(newTarget != null ? newTarget.name : "null")}");
    }

    public Transform FindNearestTarget()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameManager instance is null!");
            return null;
        }

        if (isAlly)
        {
            var enemies = GameManager.Instance.enemies;
            if (enemies == null || enemies.Count == 0)
            {
                Debug.LogWarning($"[{gameObject.name}] No enemies found for ally to target!");
                return null;
            }
            Debug.Log($"[{gameObject.name}] Finding nearest enemy, enemy count: {enemies.Count}");
            float minDist = float.MaxValue;
            Transform nearest = null;
            foreach (var enemy in enemies)
            {
                if (enemy == null || !enemy.activeInHierarchy)
                {
                    Debug.Log($"[{gameObject.name}] Enemy skipped: {(enemy == null ? "null" : enemy.name + " (inactive)")}");
                    continue;
                }
                if (enemy == gameObject)
                {
                    Debug.Log($"[{gameObject.name}] Skipping self as target: {enemy.name}");
                    continue;
                }
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                Debug.Log($"[{gameObject.name}] Checking enemy: {enemy.name}, distance: {dist}, position: {enemy.transform.position}, layer: {LayerMask.LayerToName(enemy.layer)}");
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy.transform;
                }
            }
            Debug.Log($"[{gameObject.name}] Nearest enemy: {(nearest != null ? nearest.name : "none")}, distance: {(nearest != null ? minDist.ToString() : "N/A")}");
            return nearest;
        }
        else
        {
            var players = GameManager.Instance.players;
            if (players == null || players.Count == 0)
            {
                Debug.LogWarning($"[{gameObject.name}] No players found for enemy to target!");
                return null;
            }
            Debug.Log($"[{gameObject.name}] Finding nearest player, player count: {players.Count}");
            float minDist = float.MaxValue;
            Transform nearest = null;
            foreach (var player in players)
            {
                if (player == null || !player.activeInHierarchy) continue;
                float dist = Vector3.Distance(transform.position, player.transform.position);
                Debug.Log($"[{gameObject.name}] Checking player: {player.name}, distance: {dist}, position: {player.transform.position}");
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = player.transform;
                }
            }
            Debug.Log($"[{gameObject.name}] Nearest player: {(nearest != null ? nearest.name : "none")}");
            return nearest;
        }
    }
}