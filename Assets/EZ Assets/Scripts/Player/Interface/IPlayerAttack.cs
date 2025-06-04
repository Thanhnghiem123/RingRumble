using System.Collections;
using UnityEngine;

public interface IPlayerAttack
{
    bool IsBlocking();
    bool Block();
    bool UnBlock();
    void Punch(HitType hitType);
    void HoldPunch(HitType hitType);

    void Kick(HitType hitType);
    void HoldKick(HitType hitType);



}