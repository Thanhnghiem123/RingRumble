using UnityEngine;

/// <summary>
/// Quản lý trạng thái và animation của enemy.
/// </summary>
public class Enemy : MonoBehaviour
{
    private IAnimationManager animationManager;
    private EnemyMovement enemyMovement;
    private bool lastIsMoving = false;

    void Start()
    {
        animationManager = GetComponent<IAnimationManager>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

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
