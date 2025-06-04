using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static List<LevelData> GenerateLevels(int totalLevels)
    {
        var levels = new List<LevelData>();
        for (int i = 1; i <= totalLevels; i++)
        {
            // Example: Increase enemy count and health per level
            int enemyCount = 2 + i; // Start with 3, increase by 1 each level
            float enemyHealth = 50f + i * 15f; // Start with 65, increase by 15 each level
            float enemySpeed = 1f + i * 0.1f; // Start with 1, increase by 0.1 each level

            levels.Add(new LevelData(i, enemyCount, enemyHealth, enemySpeed));
        }
        return levels;
    }
}