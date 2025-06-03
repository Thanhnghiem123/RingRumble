using UnityEngine;

public interface IEnemyMovement
{
    bool IsMoving { get; }
    void SetDestination(Vector3 position);
}