using UnityEngine;
using Ilumisoft.HealthSystem;

public class Ally : MonoBehaviour
{
    private IAnimationManager animationManager;
    private IEnemyMovement allyMovement;
    private IEnemyAttack allyAttack;
    private Health healthAlly;

    void Start()
    {
        animationManager = GetComponent<IAnimationManager>();
        allyMovement = GetComponent<IEnemyMovement>();
        allyAttack = GetComponent<IEnemyAttack>();
        healthAlly = GetComponent<Health>();

        StartLevel();
    }

    void Update()
    {
        if (allyMovement == null || animationManager == null)
            return;

        if (allyAttack.IsPlayerInAttackRange())
        {
            allyAttack.Attack();
            allyMovement.Stop();
        }
        else
        {
            allyMovement.Movement();
        }
    }

    public void StartLevel()
    {
        LevelData data = GameManager.Instance.GetLevelData();
        if (data == null)
        {
            Debug.LogError("LevelData is null. Cannot start level.");
            return;
        }

        // Gán máu cho Ally
        if (healthAlly != null)
        {
            healthAlly.MaxHealth = data.playerHealth;
            healthAlly.SetHealth(data.playerHealth);
        }

        // Gán các chỉ số tấn công cho AllyAttack
        if (allyAttack != null)
        {
            allyAttack.DamePunch = data.playerDamePunch;
            allyAttack.DameHoldPunch = data.playerDameHoldPunch;
            allyAttack.DameKick = data.playerDameKick;
            allyAttack.DameHoldKick = data.playerDameHoldKick;
            // Nếu muốn, có thể set thêm các chỉ số khác
        }

        // Gán tốc độ di chuyển cho AllyMovement
        if (allyMovement != null)
        {
            allyMovement.Speed = data.playerSpeed;
        }
    }
}
