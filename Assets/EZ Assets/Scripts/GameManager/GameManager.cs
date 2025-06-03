using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameMode gameMode;
    public List<Player> players = new List<Player>();
    public List<Enemy> enemies = new List<Enemy>();

    [Header("Spawn Settings")]
    [Range(1, 10)]
    public int playerCount = 1;
    [Range(1, 10)]
    public int enemyCount = 1;

    // Singleton pattern nếu cần
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Khởi tạo player/enemy theo mode
        SetupGameMode();
    }

    void SetupGameMode()
    {
        switch (gameMode)
        {
            case GameMode.OneVsOne:
                playerCount = 1;
                enemyCount = 1;
                break;
            case GameMode.OneVsMany:
                playerCount = 1;
                // enemyCount giữ nguyên theo Inspector
                break;
            case GameMode.ManyVsMany:
                // playerCount và enemyCount giữ nguyên theo Inspector
                break;
        }
        // Ở đây bạn sẽ thêm logic spawn player/enemy dựa trên playerCount và enemyCount
    }
}
