using UnityEngine;

public interface IEnemyAttack
{
    float DamePunch { get; set; }
    float DameHoldPunch { get; set; }
    float DameKick { get; set; }
    float DameHoldKick { get; set; }
    float StoppingDistance { get; set; }
    float CapsuleHeight { get; set; }
    float CapsuleRadius { get; set; }
    float AttackCooldown { get; set; }
    float AttackDelay { get; set; }
    bool RandomizeAttacks { get; set; }
    float HoldAttackChance { get; set; }
    float KickChance { get; set; }


    void PerformAttack();

    bool IsPlayerInAttackRange();
    void Attack();
}
