using System.Collections;
using UnityEngine;

public interface IPlayerAttack
{
    bool IsBlocking();
    void Punch();
    void HoldPunch();

    void Kick();
    void HoldKick();
    bool Block();
    bool UnBlock();

    void Jump();
    void JumpOverIntro();
}