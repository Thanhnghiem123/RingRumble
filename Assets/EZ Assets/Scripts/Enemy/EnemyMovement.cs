using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour, IEnemyMovement
{
    private NavMeshAgent agent;
    private Transform targetPlayer;
    private IAnimationManager animationManager;

    public bool isMoving = false;
    public bool IsMoving => isMoving;
    public float speed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animationManager = GetComponent<IAnimationManager>();
    }

    public void Movement()
    {
        if (targetPlayer == null)
            targetPlayer = FindNearestPlayer();

        if (targetPlayer != null)
        {
            if(IsMoving)
            {
                agent.SetDestination(targetPlayer.position);
            }
            speed = agent.velocity.magnitude;
            isMoving = speed > 0.2f;
        }
        else
        {
            Stop();
        }

        if (IsMoving)
        {
            animationManager.PlayRun(IsMoving);
            if (!IsMoving)
                animationManager.PlayIdle();
        }
    }

    public void Stop()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            isMoving = false;
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
        foreach (var player in GameManager.Instance.players)
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
