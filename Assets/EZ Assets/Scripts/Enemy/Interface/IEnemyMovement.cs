using UnityEngine;

public interface IEnemyMovement
{
    bool IsMoving { get; }
    void SetTarget(Transform target);
    Transform FindNearestPlayer();

    void Movement();
    void Stop();
}
