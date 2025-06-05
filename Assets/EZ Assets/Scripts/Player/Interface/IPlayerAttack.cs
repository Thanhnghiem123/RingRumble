using System.Collections;
using UnityEngine;

public interface IPlayerAttack
{
    float DamePunch { get; set; }
    float DameHoldPunch { get; set; }
    float DameKick { get; set; }
    float DameHoldKick { get; set; }
    float AttackCooldown { get; set; }

    bool IsBlocking();
    bool Block();
    bool UnBlock();
    void Punch(HitType hitType);
    void HoldPunch(HitType hitType);
    void Kick(HitType hitType);
    void HoldKick(HitType hitType);
}
