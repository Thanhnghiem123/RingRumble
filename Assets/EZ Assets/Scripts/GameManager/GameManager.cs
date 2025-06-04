using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameMode gameMode;
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public List<GameObject> enemyPrefab;

    [Header("Spawn Points")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public float playerSpawnRadius = 1f; // Bán kính spawn player
    public float enemySpawnRadius = 1f;  // Bán kính spawn enemy

    [Header("Spawn Settings")]
    [Range(0, 10)]
    public int playerCount = 1;
    [Range(1, 10)]
    public int enemyCount = 1;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerCount = playerCount - 1;
        SetupGameMode();
        SpawnPlayersInArea(playerCount);
        SpawnEnemiesInArea(enemyCount);
        

    }

    void SetupGameMode()
    {
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
            playerObj.SetActive(true); // Đảm bảo player được kích hoạt
            //Player playerComponent = playerObj.GetComponent<Player>();
            //if (playerComponent != null)
            //{
                players.Add(playerObj);
            //}
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
            //Enemy enemyComponent = enemyObj.GetComponent<Enemy>();
            //if (enemyComponent != null)
            //{
            enemies.Add(enemyObj);
            //}
        }
    }

    public void RemovePlayer(GameObject player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }

}
