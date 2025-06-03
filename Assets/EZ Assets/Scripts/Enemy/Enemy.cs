using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Quản lý trạng thái và animation của enemy.
/// </summary>
public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IAnimationManager animationManager;
    private EnemyMovement enemyMovement;
    private bool lastIsMoving = false;

    void Start()
    {

        animationManager = GetComponent<IAnimationManager>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        if (enemyMovement == null || animationManager == null)
            return;

        // Kiểm tra trạng thái di chuyển và gọi animation tương ứng
        if (enemyMovement.IsMoving != lastIsMoving)
        {
            lastIsMoving = enemyMovement.IsMoving;
            animationManager.PlayRun(lastIsMoving);
            if (!lastIsMoving)
                animationManager.PlayIdle();
        }
    }
}