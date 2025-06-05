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
    private IEnemyAttack enemyAttack;

    void Start()
    {

        animationManager = GetComponent<IAnimationManager>();
        enemyMovement = GetComponent<IEnemyMovement>();
        enemyAttack = GetComponent<IEnemyAttack>();
    }

    // Update is called once per frame
    void Update()
    {

        if (enemyMovement == null || animationManager == null)
            return;

        if (enemyAttack.IsPlayerInAttackRange())
        {
            enemyAttack.Attack();
            enemyMovement.Stop();
        }

        else
            enemyMovement.Movement();
        //Debug.Log("Enemy is moving: " + enemyMovement.IsMoving);




    }
}