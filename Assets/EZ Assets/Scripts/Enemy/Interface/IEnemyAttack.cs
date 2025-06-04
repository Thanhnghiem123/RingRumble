using UnityEngine;

public interface IEnemyAttack
{
    float damePunch { get; set; }
    float dameHoldPunch { get; set; }
    float dameKick { get; set; }
    float dameHoldKick { get; set; }
    float attackCooldown { get; set; }
    float attackDelay { get; set; }
    bool randomizeAttacks { get; set; }
    float holdAttackChance { get; set; }
    float kickChance { get; set; }

    void PerformAttack();
}
