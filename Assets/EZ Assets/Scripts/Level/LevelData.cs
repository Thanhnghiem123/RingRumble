using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [Range(1, 10)]
    public int levelData;
    public float enemyHealth;
    public float enemyDamePunch;
    public float enemyDameKick;
    public float enemySpeed;
    public float enemyAttackCooldown;

    public float playerHealth;
    public float playerDamePunch;
    public float playerDameKick;
    public float playerSpeed;
    public float playerAttackCooldown;

    public LevelData(int levelData, float enemyHealth, float enemyDamePunch, float enemyDameKick,
                     float enemySpeed, float enemyAttackCooldown,
                     float playerHealth, float playerDamePunch, float playerDameKick,
                     float playerSpeed, float playerAttackCooldown)
    {
        this.levelData = levelData;
        this.enemyHealth = enemyHealth;
        this.enemyDamePunch = enemyDamePunch;
        this.enemyDameKick = enemyDameKick;
        this.enemySpeed = enemySpeed;
        this.enemyAttackCooldown = enemyAttackCooldown;
        this.playerHealth = playerHealth;
        this.playerDamePunch = playerDamePunch;
        this.playerDameKick = playerDameKick;
        this.playerSpeed = playerSpeed;
        this.playerAttackCooldown = playerAttackCooldown;
    }
}





///
/// health (min: 100)
/// speed (min: 0.5
/// dame (punch, holdpunch, kick, holdkick)
/// attackCooldown (max = 5
///
