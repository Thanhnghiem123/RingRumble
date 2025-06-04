using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Quản lý trạng thái và animation của enemy.
/// </summary>
public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IAnimationManager animationManager;
    private IEnemyMovement enemyMovement;
    private bool lastIsMoving = false;

    void Start()
    {

        animationManager = GetComponent<IAnimationManager>();
        enemyMovement = GetComponent<IEnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        if (enemyMovement == null || animationManager == null)
            return;
        enemyMovement.Movement();

        // Kiểm tra trạng thái di chuyển và gọi animation tương ứng
        
    }
}