using System.Collections.Generic;
using UnityEngine;



public class LevelGenerator : MonoBehaviour
{
    public float baseEnemyHealth = 100f;
    public float baseEnemyDamePunch = 2f;
    public float baseEnemyDameHoldPunch = 3f;
    public float baseEnemyDameKick = 4f;
    public float baseEnemyDameHoldKick = 5f;
    public float baseEnemySpeed = 0.5f;
    public float baseEnemyAttackCooldown = 5f;
    public float baseEnemyHoldAttackChance = 0.1f;
    public float baseEnemyKickChance = 0.1f;

    public float basePlayerHealth = 100f;
    public float basePlayerDamePunch = 5f;
    public float basePlayerDameHoldPunch = 10f;
    public float basePlayerDameKick = 7f;
    public float basePlayerDameHoldKick = 15f;
    public float basePlayerSpeed = 1f;

    public float enemyHealthMultiplier;
    public float enemyDamageMultiplier;
    public float enemySpeedMultiplier;
    public float cooldownReductionMultiplier;
    public float chanceMultiplier;

    public float playerHealthMultiplier;
    public float playerDamageMultiplier;
    public float playerSpeedMultiplier;


    public float percentDameEnemy = 0.25f; 
    public float percentDamePlayer = 0.25f; 



    public List<LevelData> GenerateLevels(int levelAmountData, GameMode gameMode)
    {
        var levels = new List<LevelData>();
        

        switch (gameMode)
        {
            case GameMode.OneVsMany:
                enemyHealthMultiplier = 0.8f;       
                enemyDamageMultiplier = 0.8f;       
                enemySpeedMultiplier = 0.8f;       
                cooldownReductionMultiplier = 0.5f; 
                chanceMultiplier = 0.5f;            
                playerHealthMultiplier = 1.3f;     
                playerDamageMultiplier = 1.3f;      
                playerSpeedMultiplier = 1.3f;       
                break;
            case GameMode.ManyVsMany:
                enemyHealthMultiplier = 1.3f;       
                enemyDamageMultiplier = 1.3f;      
                enemySpeedMultiplier = 1.3f;        
                cooldownReductionMultiplier = 1.5f; 
                chanceMultiplier = 1.5f;            
                playerHealthMultiplier = 0.9f;      
                playerDamageMultiplier = 0.9f;     
                playerSpeedMultiplier = 0.9f;      
                break;
            case GameMode.OneVsOne:
            default:
                enemyHealthMultiplier = 1f;         
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
            float enemyHealth = baseEnemyHealth * enemyHealthMultiplier * (1 + 0.3f * (i - 1));
            float enemyDamePunch = baseEnemyDamePunch * enemyDamageMultiplier * CalculateDame(i, percentDameEnemy);
            float enemyDameHoldPunch = baseEnemyDameHoldPunch * enemyDamageMultiplier * CalculateDame(i, percentDameEnemy);
            float enemyDameKick = baseEnemyDameKick * enemyDamageMultiplier * CalculateDame(i, percentDameEnemy);
            float enemyDameHoldKick = baseEnemyDameHoldKick * enemyDamageMultiplier * CalculateDame(i, percentDameEnemy);
            float enemySpeed = (baseEnemySpeed + 0.1f * (i - 1)) * enemySpeedMultiplier;
            float enemyAttackCooldown = Mathf.Max(1.0f, baseEnemyAttackCooldown - 0.25f * (i - 1) * cooldownReductionMultiplier);
            float enemyHoldAttackChance = Mathf.Min(1f, baseEnemyHoldAttackChance + 0.05f * (i - 1) * chanceMultiplier);
            float enemyKickChance = Mathf.Min(1f, baseEnemyKickChance + 0.05f * (i - 1) * chanceMultiplier);

            float playerHealth = basePlayerHealth * playerHealthMultiplier + 5f * (i - 1);
            float playerDamePunch = basePlayerDamePunch * playerDamageMultiplier + CalculateDame(i, percentDamePlayer);
            float playerDameHoldPunch = basePlayerDameHoldPunch * playerDamageMultiplier + CalculateDame(i, percentDamePlayer);
            float playerDameKick = basePlayerDameKick * playerDamageMultiplier + CalculateDame(i, percentDamePlayer);
            float playerDameHoldKick = basePlayerDameHoldKick * playerDamageMultiplier + CalculateDame(i, percentDamePlayer);
            float playerSpeed = (basePlayerSpeed + 0.15f * (i - 1)) * playerSpeedMultiplier;

            Debug.Log($"[Level {i}] [Mode: {gameMode}] Enemy: Health={enemyHealth:F1}, Punch={enemyDamePunch:F1}, HoldPunch={enemyDameHoldPunch:F1}, Kick={enemyDameKick:F1}, HoldKick={enemyDameHoldKick:F1}, Speed={enemySpeed:F1}, cooldownReductionMultiplier={enemyAttackCooldown:F1}, HoldChance={enemyHoldAttackChance:F1}, KickChance={enemyKickChance:F1}");

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

    private float CalculateDame(int level, float percent)
    {
        int levelIndex = level - 1;
        return 1 + percent * levelIndex;
    }

}