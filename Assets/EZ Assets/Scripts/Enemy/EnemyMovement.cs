using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
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

    Transform FindNearestPlayer()
    {
        // Đảm bảo GameManager và danh sách player hợp lệ
        if (GameManager.Instance == null || GameManager.Instance.players == null || GameManager.Instance.players.Count == 0)
            return null;
        Debug.Log("Target Player: " + (targetPlayer != null ? targetPlayer.position : "None"));
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
