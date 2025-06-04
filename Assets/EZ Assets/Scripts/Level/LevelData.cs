using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public int LevelNumber;

    public int EnemyCount;
    public float EnemyHealth;
    public float EnemySpeed;
    // Add more parameters as needed

    public LevelData(int levelNumber, int enemyCount, float enemyHealth, float enemySpeed)
    {
        LevelNumber = levelNumber;
        EnemyCount = enemyCount;
        EnemyHealth = enemyHealth;
        EnemySpeed = enemySpeed;
    }
}

