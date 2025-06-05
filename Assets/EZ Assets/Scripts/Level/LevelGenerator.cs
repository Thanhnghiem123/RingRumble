using System.Collections.Generic;
using Ilumisoft.HealthSystem;
using UnityEngine;

public static class LevelGenerator
{
    //public static IPlayerAttack playerAttack;
    //public static IPlayerMovement playerMovement;
    //public static IEnemyAttack enemyAttack;
    //public static IEnemyMovement enemyMovement;
    //public static Health healthPlayer;
    //public static HealthEnemy healthEnemy;



    public static List<LevelData> GenerateLevels(int levelAmountData)
    {
        var levels = new List<LevelData>();

        float baseEnemyHealth = 100f;
        float baseEnemyDamePunch = 2f;
        float baseEnemyDameHoldPunch = 3f;
        float baseEnemyDameKick = 4f;
        float baseEnemyDameHoldKick = 5f;
        float baseEnemySpeed = 0.5f;
        float baseEnemyAttackCooldown = 5f;
        float baseEnemyHoldAttackChance = 0.1f;
        float baseEnemyKickChance = 0.1f;

        float basePlayerHealth = 100f;
        float basePlayerDamePunch = 5f;
        float basePlayerDameHoldPunch = 10f;
        float basePlayerDameKick = 7f;
        float basePlayerDameHoldKick = 15f;
        float basePlayerSpeed = 1f;

        for (int i = 1; i <= levelAmountData; i++)
        {
            float enemyHealth = baseEnemyHealth * (1 + 0.25f * (i - 1));
            float enemyDamePunch = baseEnemyDamePunch * (1 + 0.25f * (i - 1));
            float enemyDameHoldPunch = baseEnemyDameHoldPunch * (1 + 0.25f * (i - 1));
            float enemyDameKick = baseEnemyDameKick * (1 + 0.25f * (i - 1));
            float enemyDameHoldKick = baseEnemyDameHoldKick * (1 + 0.25f * (i - 1));
            float enemySpeed = baseEnemySpeed + 0.1f * (i - 1);
            float enemyAttackCooldown = Mathf.Max(1.0f, baseEnemyAttackCooldown - 0.25f * (i - 1));
            float enemyHoldAttackChance = Mathf.Min(1f, baseEnemyHoldAttackChance + 0.05f * (i - 1));
            float enemyKickChance = Mathf.Min(1f, baseEnemyKickChance + 0.05f * (i - 1));

            float playerHealth = basePlayerHealth + 5f * (i - 1);
            float playerDamePunch = basePlayerDamePunch + 0.5f * (i - 1);
            float playerDameHoldPunch = basePlayerDameHoldPunch + 0.5f * (i - 1);
            float playerDameKick = basePlayerDameKick + 0.5f * (i - 1);
            float playerDameHoldKick = basePlayerDameHoldKick + 0.5f * (i - 1);
            float playerSpeed = basePlayerSpeed + 0.1f * (i - 1);

            // Debug log for enemy
            Debug.Log($"[Level {i}] Enemy: Health={enemyHealth}, Punch={enemyDamePunch}, HoldPunch={enemyDameHoldPunch}, Kick={enemyDameKick}, HoldKick={enemyDameHoldKick}, Speed={enemySpeed}, Cooldown={enemyAttackCooldown}, HoldChance={enemyHoldAttackChance}, KickChance={enemyKickChance}");

            // Debug log for player (zombie)
            Debug.Log($"[Level {i}] Player: Health={playerHealth}, Punch={playerDamePunch}, HoldPunch={playerDameHoldPunch}, Kick={playerDameKick}, HoldKick={playerDameHoldKick}, Speed={playerSpeed}");

            levels.Add(new LevelData(
                enemyHealth, enemyDamePunch, enemyDameHoldPunch, enemyDameKick, enemyDameHoldKick,
                enemySpeed, enemyAttackCooldown, enemyHoldAttackChance, enemyKickChance,
                playerHealth, playerDamePunch, playerDameHoldPunch, playerDameKick, playerDameHoldKick,
                playerSpeed
            ));
            
        }
        return levels;
    }
}