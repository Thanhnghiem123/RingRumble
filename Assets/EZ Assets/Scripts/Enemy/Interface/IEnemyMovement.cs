using UnityEngine;

public interface IEnemyMovement
{
    bool IsMoving { get; }
    float Speed { get; set; }
    void SetTarget(Transform target);
    Transform FindNearestTarget();

    void Movement();
    void Stop();

    float SetAgentSpeed { get; set; }
}
