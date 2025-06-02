using UnityEngine;
public interface IMovementInput
{
    void ProcessInput();
    bool IsMoving();
    Vector3 GetDirection();
}