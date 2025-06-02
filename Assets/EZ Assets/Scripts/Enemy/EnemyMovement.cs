using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Điều khiển di chuyển của enemy, không xử lý animation.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;

    private bool isMoving = false;
    public bool IsMoving => isMoving; // Cho phép class khác đọc trạng thái di chuyển

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);

            float speed = agent.velocity.magnitude;
            isMoving = speed > 0.2f;
        }
    }
}
