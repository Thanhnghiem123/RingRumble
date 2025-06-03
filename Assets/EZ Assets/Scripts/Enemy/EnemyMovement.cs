using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Điều khiển di chuyển của enemy, không xử lý animation.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour, IEnemyMovement
{
    private NavMeshAgent agent;

    private bool isMoving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    public bool IsMoving => isMoving; // Cho phép class khác đọc trạng thái di chuyển



    public void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);

        float speed = agent.velocity.magnitude;
        isMoving = speed > 0.2f;
    }

    
}