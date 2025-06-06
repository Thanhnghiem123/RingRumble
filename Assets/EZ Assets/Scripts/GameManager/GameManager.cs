using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Cài đặt Game")]
    [Range(1, 10)]
    public int totalLevels = 10;
    [Range(1, 3)]
    public int currentMode = 1;
    [Range(1, 10)]
    public int currentLevel = 1;

    [Header("Chế độ chơi")]
    public GameMode gameMode;
    public LevelData levelData; 
    public List<LevelData> levels;

    [Header("Đối tượng trong game")]
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();

    [Header("Prefab")]
    public GameObject playerPrefab;
    public List<GameObject> enemyPrefab;

    [Header("Điểm xuất hiện")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public float playerSpawnRadius = 1f;
    public float enemySpawnRadius = 1f;

    [Header("Cài đặt xuất hiện")]
    [Range(1, 10)]
    public int playerCount = 1;
    [Range(1, 10)]
    public int enemyCount = 1;

    [Header("Tạo màn chơi")]
    public LevelGenerator levelGenerator; 

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        levelGenerator = GetComponent<LevelGenerator>();
        Debug.Log("GameManager Awake called" + gameMode);
  
    }

    void Start()
    {
        playerCount = playerCount - 1;
        currentMode = PlayerPrefs.GetInt("GameMode", (int)gameMode);
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        gameMode = (GameMode)currentMode;
        levels = levelGenerator.GenerateLevels(totalLevels, gameMode); // Gọi trực tiếp từ LevelGenerator

        SetupGameMode(currentMode);
        SpawnPlayersInArea(playerCount);
        SpawnEnemiesInArea(enemyCount);
    }

    public LevelData GetLevelData()
    {
        if (currentLevel < 1 || currentLevel > levels.Count)
            return null;
        levelData = levels[currentLevel - 1];
        Debug.Log($"Current Level: {currentLevel}, Enemy Health: {levelData.enemyHealth}, Player Health: {levelData.playerHealth}");
        return levelData;
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();

    }

    public void SetupGameMode(int gameModenew)
    {
        gameMode = (GameMode)gameModenew;
        PlayerPrefs.SetInt("GameMode", (int)gameMode);
        PlayerPrefs.Save();
        Debug.Log("mode: " + gameMode);
        switch (gameMode)
        {
            case GameMode.OneVsOne:
                playerCount = 0;
                enemyCount = 1;
                break;
            case GameMode.OneVsMany:
                playerCount = 0;
                break;
            case GameMode.ManyVsMany:
                break;
        }
    }

    void SpawnPlayersInArea(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = Random.insideUnitCircle * playerSpawnRadius;
            Vector3 spawnPos = playerSpawnPoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);
            GameObject playerObj = Instantiate(playerPrefab, spawnPos, playerSpawnPoint.rotation);
            playerObj.SetActive(true);
            players.Add(playerObj);
        }
    }

    void SpawnEnemiesInArea(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * enemySpawnRadius;
            Vector3 spawnPos = enemySpawnPoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);
            int randomIndex = Random.Range(0, enemyPrefab.Count);
            GameObject prefab = enemyPrefab[randomIndex];
            GameObject enemyObj = Instantiate(prefab, spawnPos, enemySpawnPoint.rotation);
            enemies.Add(enemyObj);
        }
    }

    public void RemovePlayer(GameObject player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    public void LoadSceneAfterDelay(string sceneName, float delay)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, delay));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public bool CheckEnemyCount()
    {
        return enemies.Count > 0;
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }
}