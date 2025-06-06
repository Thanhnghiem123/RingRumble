using Ilumisoft.HealthSystem;
using Unity.VisualScripting;
using UnityEngine;


public class Enemy : MonoBehaviour
{

    [Tooltip("State of player which to check can player attack")]
    public bool normalState = true;

    private IAnimationManager animationManager;
    private IEnemyMovement enemyMovement;
    private IEnemyAttack enemyAttack;
    private HealthEnemy healthEnemy;

    void Start()
    {

        animationManager = GetComponent<IAnimationManager>();
        enemyMovement = GetComponent<IEnemyMovement>();
        enemyAttack = GetComponent<IEnemyAttack>();
        healthEnemy = GetComponent<HealthEnemy>();


        StartLevel();
    }

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




    }


    public void StartLevel()
    {
        LevelData data = GameManager.Instance.GetLevelData();
        if (data == null)
        {
            Debug.LogError("LevelData is null. Cannot start level.");
            return;
        }

        if (healthEnemy != null)
        {
            healthEnemy.MaxHealth = data.enemyHealth;
            healthEnemy.SetHealth(data.enemyHealth);
            Debug.Log("Enemy health set to: " + healthEnemy.MaxHealth);
        }

        if (enemyAttack != null)
        {
            enemyAttack.DamePunch = data.enemyDamePunch;
            enemyAttack.DameHoldPunch = data.enemyDameHoldPunch;
            enemyAttack.DameKick = data.enemyDameKick;
            enemyAttack.DameHoldKick = data.enemyDameHoldKick;
            enemyAttack.AttackCooldown = data.enemyAttackCooldown;
            enemyAttack.HoldAttackChance = data.enemyholdAttackChance;
            enemyAttack.KickChance = data.enemykickChance;

            
            Debug.Log("Enemy attack stats set: " +
                $"DamePunch={enemyAttack.DamePunch}, " +
                $"DameHoldPunch={enemyAttack.DameHoldPunch}, " +
                $"DameKick={enemyAttack.DameKick}, " +
                $"DameHoldKick={enemyAttack.DameHoldKick}, " +
                $"AttackCooldown={enemyAttack.AttackCooldown}, " +
                $"HoldAttackChance={enemyAttack.HoldAttackChance}, " +
                $"KickChance={enemyAttack.KickChance}");
        }

        if (enemyMovement != null)
        {
            enemyMovement.SetAgentSpeed = data.enemySpeed;
        }

        Debug.Log("STARTTTTTTTTT");
    }

}