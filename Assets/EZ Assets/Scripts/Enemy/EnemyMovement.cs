using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour, IEnemyMovement
{
    private NavMeshAgent agent;
    private Transform targetPlayer;
    private IAnimationManager animationManager;
    private IEnemyAttack enemyAttack;

    public bool isMoving = false;
    public float speed;

    public bool IsMoving => isMoving;

    public float Speed { get => speed; set => speed = value; }

    // Fix for CS0201 and CS0029: Change the property type from void to float
    public float SetAgentSpeed
    {
        get => agent.speed;
        set => agent.speed = value;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animationManager = GetComponent<IAnimationManager>();
        enemyAttack = GetComponent<IEnemyAttack>();
    }

    public void Movement()
    {
        //if (!AttackManager.ENormalState) return;
        
            targetPlayer = FindNearestPlayer();

        if (targetPlayer != null)
        {
            agent.SetDestination(targetPlayer.position);

            speed = agent.velocity.magnitude;
            isMoving = speed > 0.2f;

            animationManager.PlayRun(IsMoving);
        }
    }

    public void Stop()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            isMoving = false;
            animationManager.PlayRun(IsMoving);
        }
    }

    public void SetTarget(Transform target)
    {
        targetPlayer = target;
    }

    public Transform FindNearestPlayer()
    {
        if (GameManager.Instance == null || GameManager.Instance.players == null || GameManager.Instance.players.Count == 0)
            return null;

        float minDist = float.MaxValue;
        Transform nearest = null;
        foreach (var player in GameManager.Instance.enemies)
        {
            if (player == null) continue;
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = player.transform;
            }
        }
        return nearest;
    }
}

