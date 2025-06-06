using System.Collections;
using Ilumisoft.HealthSystem;
using UnityEngine;
public class Player : MonoBehaviour
{

    [Header("Cooldown Settings")]
    [Tooltip("Cooldown time (seconds) between punches")]
    public float punchCooldown;
    [Tooltip("Cooldown time (seconds) between kicks")]
    public float kickCooldown;
    [Tooltip("Cooldown time (seconds) between jumps")]
    public float jumpCooldown;
    [Tooltip("Cooldown time (seconds) between climbs")]
    public float climbCooldown;
    [Tooltip("Cooldown time (seconds) between special attacks")]
    public float specialAttackCooldown;




    private CooldownTimer punchTimer;
    private CooldownTimer kickTimer;
    private CooldownTimer jumpTimer;
    private CooldownTimer specialTimer;
    private CooldownTimer climbTimer;


    private IAnimationManager animationManager;
    private IPlayerAttack playerAttack;
    private IPlayerMovement playerMovement;
    private IMovementInput movementInput;
    private Health healthPlayer;
    private bool isInitialized = false;
   

 



    void Start()
    {
        animationManager = GetComponent<IAnimationManager>();
        if (animationManager == null)
        {
            Debug.LogError("AnimationManager not found on " + gameObject.name);
        }

        playerAttack = GetComponent<IPlayerAttack>();
        if (playerAttack == null)
        {
            Debug.LogError("PlayerAttack component not found on " + gameObject.name);
        }
        playerMovement = GetComponent<IPlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on " + gameObject.name);
        }
        movementInput = GetComponent<IMovementInput>();
        if (movementInput == null)
        {
            Debug.LogError("MovementInput component not found on " + gameObject.name);
        }
        healthPlayer = GetComponent<Health>();
        if (healthPlayer == null)
        {
            Debug.LogError("Health component not found on " + gameObject.name);
        }

        isInitialized = (movementInput != null);
        if (!isInitialized)
        {
            Debug.LogError("PlayerController initialization failed. Disabling script.");
            enabled = false;
        }
        

        punchTimer = new CooldownTimer(punchCooldown);
        kickTimer = new CooldownTimer(kickCooldown);
        jumpTimer = new CooldownTimer(jumpCooldown);
        specialTimer = new CooldownTimer(specialAttackCooldown);
        climbTimer = new CooldownTimer(climbCooldown); // Khởi tạo timer leo trèo

        Debug.Log("PlayerAttack initialized with cooldowns: " +
            $"Punch: {punchCooldown}s, Kick: {kickCooldown}s, Jump: {jumpCooldown}s, Special: {specialAttackCooldown}s, Climb: {climbCooldown}s");
    


        StartLevel();

    }

    public void StartLevel()
    {
        LevelData data = GameManager.Instance.GetLevelData();
        if (data == null)
        {
            Debug.LogError("LevelData is null. Cannot start level.");
            return;
        }

        // Assign health to Player
        if (healthPlayer != null)
        {
            healthPlayer.MaxHealth = data.playerHealth;
            healthPlayer.SetHealth(data.playerHealth);
            Debug.Log("Player health set to: " + healthPlayer.MaxHealth);
        }

        // Assign attack stats to PlayerAttack
        if (playerAttack != null)
        {
            playerAttack.DamePunch = data.playerDamePunch;
            playerAttack.DameHoldPunch = data.playerDameHoldPunch;
            playerAttack.DameKick = data.playerDameKick;
            playerAttack.DameHoldKick = data.playerDameHoldKick;

            Debug.Log("Player attack stats set: " +
                $"DamePunch={playerAttack.DamePunch}, " +
                $"DameHoldPunch={playerAttack.DameHoldPunch}, " +
                $"DameKick={playerAttack.DameKick}, " +
                $"DameHoldKick={playerAttack.DameHoldKick}");
        }

        // Assign movement speed to PlayerMovement
        if (playerMovement != null)
        {
            playerMovement.Speed = data.playerSpeed;
        }

        Debug.Log("Player START LEVEL");
    }

    void Update()
    {
        if (!isInitialized) return;

        playerMovement.IsGrounded(); // Cập nhật trạng thái chạm đất

        movementInput.ProcessInput();
        if (movementInput.IsMoving())
        {
            Move();
        }
        else
        {
            Stop();
        }
    }

    public void Punch()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!specialTimer.IsReady) return; // Prevent punch during special cooldown
        if (!punchTimer.IsReady) return;
        HitType hitType = (HitType)(animationManager?.PlayHeadPunch());
        punchTimer.Trigger();

        playerAttack.Punch(hitType);
    }






    public void HoldPunch()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!specialTimer.IsReady) return;
        HitType hitType = (HitType)(animationManager?.PlayHoldPunch());
        specialTimer.Trigger();

        playerAttack.HoldPunch(hitType);
    }

    public void Kick()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!kickTimer.IsReady) return;
        if (!specialTimer.IsReady) return;
        HitType hitType = (HitType)(animationManager?.PlayKick());
        kickTimer.Trigger();

        playerAttack.Kick(hitType);
    }

    public void HoldKick()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!specialTimer.IsReady) return;
        HitType hitType = (HitType)(animationManager?.PlayHoldKick());
        playerMovement?.Jump();
        specialTimer.Trigger();

        playerAttack.HoldKick(hitType);
    }

    public void Jump()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!specialTimer.IsReady) return;
        animationManager?.PlayJump();
        playerMovement?.Jump(); 
        jumpTimer.Trigger();
    }

    public void JumpOverIntro()
    {
        // Giảm thời gian cooldown cho nhảy qua intro
        

        if(playerMovement.CanClimb(2, 0.35f))
        {
            if (!climbTimer.IsReady) return;
            if (!specialTimer.IsReady) return;
            // Nếu có thể leo trèo, thực hiện nhảy qua intro
            animationManager?.PlayJumpOverIntro();
            playerMovement?.JumpOverIntro(); // Gọi vật lý nhảy qua intro từ PlayerMovement
            // Kích hoạt cooldown cho nhảy qua intro
            climbTimer.Trigger();
        }
        
    }

    public bool Block()
    {
        if (!climbTimer.IsReady) return false;
        if (!jumpTimer.IsReady) return false;
        if (!specialTimer.IsReady) return false;
        animationManager?.PlayBlock((bool)(playerAttack?.Block()));
        return true;
    }

    public bool UnBlock()
    {
        animationManager?.PlayBlock((bool)(playerAttack?.UnBlock()));
        return true;
    }

    public bool IsBlocking()
    {
        return playerAttack.IsBlocking();
    }

    public void Move()
    {
        if (!climbTimer.IsReady) return;
        playerMovement?.Move();
        animationManager?.PlayRun(true);
    }

    public void Stop()
    {
        playerMovement?.Stop();
    }

    public bool IsGrounded()
    {
        return playerMovement?.IsGrounded() ?? false;
    }

    public void ProcessInput()
    {
        if (!climbTimer.IsReady) return;
        movementInput?.ProcessInput();
        Debug.Log($"Player moving in directionsdasdsa:");
    }

    public bool IsMoving()
    {
        return movementInput?.IsMoving() ?? false;
    }

    public Vector3 GetDirection()
    {
        return movementInput?.GetDirection() ?? Vector3.zero;

    }












    ///////////////////////////////////////////////





}
