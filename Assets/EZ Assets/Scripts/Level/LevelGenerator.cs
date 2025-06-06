using System.Collections.Generic;
using UnityEngine;



public class LevelGenerator : MonoBehaviour


{
    public List<LevelData> GenerateLevels(int levelAmountData, GameMode gameMode)
    {
        var levels = new List<LevelData>();

        // Base stats for all modes
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

        // Multipliers based on GameMode
        float enemyHealthMultiplier, enemyDamageMultiplier, enemySpeedMultiplier, cooldownReductionMultiplier, chanceMultiplier;
        float playerHealthMultiplier, playerDamageMultiplier, playerSpeedMultiplier;

        switch (gameMode)
        {
            case GameMode.OneVsMany:
                enemyHealthMultiplier = 0.8f;       // 80% base health
                enemyDamageMultiplier = 0.7f;       // 70% base damage
                enemySpeedMultiplier = 0.8f;        // 80% base speed
                cooldownReductionMultiplier = 0.5f; // Slower cooldown reduction
                chanceMultiplier = 0.5f;            // Lower attack chance increase
                playerHealthMultiplier = 1.2f;      // 120% base health
                playerDamageMultiplier = 1.3f;      // 130% base damage
                playerSpeedMultiplier = 1.2f;       // 120% base speed
                break;
            case GameMode.ManyVsMany:
                enemyHealthMultiplier = 1.3f;       // 130% base health
                enemyDamageMultiplier = 1.5f;       // 150% base damage
                enemySpeedMultiplier = 1.2f;        // 120% base speed
                cooldownReductionMultiplier = 1.5f; // Faster cooldown reduction
                chanceMultiplier = 1.5f;            // Higher attack chance increase
                playerHealthMultiplier = 0.9f;      // 90% base health
                playerDamageMultiplier = 0.9f;      // 80% base damage
                playerSpeedMultiplier = 0.9f;       // 90% base speed
                break;
            case GameMode.OneVsOne:
            default:
                enemyHealthMultiplier = 1f;         // 100% base
                enemyDamageMultiplier = 1f;
                enemySpeedMultiplier = 1f;
                cooldownReductionMultiplier = 1f;
                chanceMultiplier = 1f;
                playerHealthMultiplier = 1f;
                playerDamageMultiplier = 1f;
                playerSpeedMultiplier = 1f;
                break;
        }

        for (int i = 1; i <= levelAmountData; i++)
        {
            // Apply base stats with mode multipliers and level progression
            float enemyHealth = baseEnemyHealth * enemyHealthMultiplier * (1 + 0.25f * (i - 1));
            float enemyDamePunch = baseEnemyDamePunch * enemyDamageMultiplier * (1 + 0.25f * (i - 1));
            float enemyDameHoldPunch = baseEnemyDameHoldPunch * enemyDamageMultiplier * (1 + 0.25f * (i - 1));
            float enemyDameKick = baseEnemyDameKick * enemyDamageMultiplier * (1 + 0.25f * (i - 1));
            float enemyDameHoldKick = baseEnemyDameHoldKick * enemyDamageMultiplier * (1 + 0.25f * (i - 1));
            float enemySpeed = (baseEnemySpeed + 0.1f * (i - 1)) * enemySpeedMultiplier;
            float enemyAttackCooldown = Mathf.Max(1.0f, baseEnemyAttackCooldown - 0.25f * (i - 1) * cooldownReductionMultiplier);
            float enemyHoldAttackChance = Mathf.Min(1f, baseEnemyHoldAttackChance + 0.05f * (i - 1) * chanceMultiplier);
            float enemyKickChance = Mathf.Min(1f, baseEnemyKickChance + 0.05f * (i - 1) * chanceMultiplier);

            float playerHealth = basePlayerHealth * playerHealthMultiplier + 5f * (i - 1);
            float playerDamePunch = basePlayerDamePunch * playerDamageMultiplier + 0.5f * (i - 1);
            float playerDameHoldPunch = basePlayerDameHoldPunch * playerDamageMultiplier + 0.5f * (i - 1);
            float playerDameKick = basePlayerDameKick * playerDamageMultiplier + 0.5f * (i - 1);
            float playerDameHoldKick = basePlayerDameHoldKick * playerDamageMultiplier + 0.5f * (i - 1);
            float playerSpeed = (basePlayerSpeed + 0.1f * (i - 1)) * playerSpeedMultiplier;

            // Debug log for enemy
            Debug.Log($"[Level {i}] [Mode: {gameMode}] Enemy: Health={enemyHealth:F1}, Punch={enemyDamePunch:F1}, HoldPunch={enemyDameHoldPunch:F1}, Kick={enemyDameKick:F1}, HoldKick={enemyDameHoldKick:F1}, Speed={enemySpeed:F1}, cooldownReductionMultiplier={enemyAttackCooldown:F1}, HoldChance={enemyHoldAttackChance:F1}, KickChance={enemyKickChance:F1}");

            // Debug log for player
            Debug.Log($"[Level {i}] [Mode: {gameMode}] Player: Health={playerHealth:F1}, Punch={playerDamePunch:F1}, HoldPunch={playerDameHoldPunch:F1}, Kick={playerDameKick:F1}, HoldKick={playerDameHoldKick:F1}, Speed={playerSpeed:F1}");

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