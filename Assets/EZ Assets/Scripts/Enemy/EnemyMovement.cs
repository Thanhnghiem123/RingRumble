using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour, IEnemyMovement
{
    private NavMeshAgent agent;
    private Transform targetPlayer;

    public bool isMoving = false;
    public bool IsMoving => isMoving;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (targetPlayer == null)
            targetPlayer = FindNearestPlayer();

        if (targetPlayer != null)
        {
            agent.SetDestination(targetPlayer.position);
            float speed = agent.velocity.magnitude;
            isMoving = speed > 0.2f;
        }
        else
        {
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
