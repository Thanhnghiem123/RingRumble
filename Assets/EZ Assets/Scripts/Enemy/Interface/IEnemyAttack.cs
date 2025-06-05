using UnityEngine;

public interface IEnemyAttack
{
  

    void PerformAttack();

    bool IsPlayerInAttackRange();
    void Attack();
}
