using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public float enemyHealth;
    public float enemySpeed;
    public float enemyDamePunch;
    public float enemyDameHoldPunch;
    public float enemyDameKick;
    public float enemyDameHoldKick;
    public float enemyAttackCooldown;
    public float enemyholdAttackChance;
    public float enemykickChance;

    public float playerHealth;
    public float playerSpeed;
    public float playerDamePunch;
    public float playerDameHoldPunch;
    public float playerDameKick;
    public float playerDameHoldKick;

    public LevelData(float enemyHealth, float enemyDamePunch, float enemyDameHoldPunch, float enemyDameKick, float enemyDameHoldKick, float enemySpeed, float enemyAttackCooldown, float enemyholdAttackChance, float enemykickChance,
                     float playerHealth, float playerDamePunch, float playerDameHoldPunch, float playerDameKick, float playerDameHoldKick, float playerSpeed)
    {
        this.enemyHealth = enemyHealth;
        this.enemySpeed = enemySpeed;
        this.enemyDamePunch = enemyDamePunch;
        this.enemyDameHoldPunch = enemyDameHoldPunch;
        this.enemyDameKick = enemyDameKick;
        this.enemyDameHoldKick = enemyDameHoldKick;
        this.enemyAttackCooldown = enemyAttackCooldown;
        this.enemyholdAttackChance = enemyholdAttackChance;
        this.enemykickChance = enemykickChance;
        this.playerHealth = playerHealth;
        this.playerSpeed = playerSpeed;
        this.playerDamePunch = playerDamePunch;
        this.playerDameHoldPunch = playerDameHoldPunch;
        this.playerDameKick = playerDameKick;
        this.playerDameHoldKick = playerDameHoldKick;
    }
}