using System.Collections;
using UnityEngine;
public interface IPlayerMovement
{

    float Speed { get; set; }
    void Move();
    void Stop();
    void Jump();
    void JumpOverIntro();
    bool IsGrounded();
    bool CanClimb(float capsuleHeight, float capsuleRadius);
}