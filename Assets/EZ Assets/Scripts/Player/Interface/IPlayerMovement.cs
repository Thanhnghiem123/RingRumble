using System.Collections;
using UnityEngine;
public interface IPlayerMovement
{
    void Move();
    void Stop();
    void Jump();
    void JumpOverIntro();
    bool IsGrounded();
}