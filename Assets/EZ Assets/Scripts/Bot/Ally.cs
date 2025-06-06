using UnityEngine;
using Ilumisoft.HealthSystem;

public class Ally : MonoBehaviour
{
    private IAnimationManager animationManager;
    private IEnemyMovement enemyMovement;
    private IEnemyAttack enemyAttack;
    private Health healthAlly;

    void Start()
    {
        animationManager = GetComponent<IAnimationManager>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        healthAlly = GetComponent<Health>();

        if (animationManager == null)
            Debug.LogError($"[{gameObject.name}] IAnimationManager not found!");
        if (enemyMovement == null)
            Debug.LogError($"[{gameObject.name}] EnemyMovement not found!");
        if (enemyAttack == null)
            Debug.LogError($"[{gameObject.name}] EnemyAttack not found!");
        if (healthAlly == null)
            Debug.LogError($"[{gameObject.name}] Health not found!");

        // Cấu hình cho bot
        if (enemyMovement is EnemyMovement movement)
            movement.isAlly = true;
        if (enemyAttack is EnemyAttack attack)
            attack.isAlly = true;

        StartLevel();
        Debug.Log($"[{gameObject.name}] Ally initialized with isAlly = true.");
    }

    void Update()
    {
        if (enemyMovement == null || animationManager == null || enemyAttack == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Missing components, cannot update!");
            return;
        }

        Transform target = (enemyMovement as EnemyMovement)?.FindNearestTarget();
        if (target != null && enemyAttack.IsEnemyInAttackRange())
        {
            Debug.Log($"[{gameObject.name}] Attacking target: {target.name} in range. transform {target.transform.position}");
            enemyAttack.Attack();
            enemyMovement.Stop();
        }
        else
        {
            Debug.Log($"[{gameObject.name}] No valid target or not in range, moving to target.");
            enemyMovement.Movement();
        }
    }

    public void StartLevel()
    {
        LevelData data = GameManager.Instance?.GetLevelData();
        if (data == null)
        {
            Debug.LogError($"[{gameObject.name}] LevelData is null!");
            return;
        }

        if (healthAlly != null)
        {
            healthAlly.MaxHealth = data.enemyHealth;
            healthAlly.SetHealth(data.enemyHealth);
            Debug.Log($"[{gameObject.name}] Ally health set to: {healthAlly.MaxHealth}");
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


        }

        if (enemyMovement != null)
        {
            enemyMovement.SetAgentSpeed = data.enemySpeed;
            Debug.Log($"[{gameObject.name}] Bot speed set to: {enemyMovement.SetAgentSpeed}");
        }
    }
}