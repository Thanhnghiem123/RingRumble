using System.Collections;
using UnityEngine;
public class Player : MonoBehaviour, IPlayerAttack, IPlayerMovement, IMovementInput
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


    [Header("Attack Settings")]
    public float attackRange;
    public LayerMask enemyLayer;


    [Header("Attack Delay Settings")]
    public float punchDelay = 0.2f;
    public float kickDelay = 0.25f;
    public float holdPunchDelay = 0.3f;
    public float holdKickDelay = 0.35f;


    private CooldownTimer punchTimer;
    private CooldownTimer kickTimer;
    private CooldownTimer jumpTimer;
    private CooldownTimer specialTimer;
    private CooldownTimer climbTimer;


    private IAnimationManager animationManager;
    private IPlayerAttack playerAttack;
    private IPlayerMovement playerMovement;
    private IMovementInput movementInput;
    private HitReceiver hitReceiver;

    // getters , setter
    public float PunchDelay { get => punchDelay; set => punchDelay = value; }
    public float KickDelay { get => kickDelay; set => kickDelay = value; }
    public float HoldPunchDelay { get => holdPunchDelay; set => holdPunchDelay = value; }
    public float HoldKickDelay { get => holdKickDelay; set => holdKickDelay = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }

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
        hitReceiver = GetComponent<HitReceiver>();
        if (hitReceiver == null)
        {
            Debug.LogError("HitReceiver component not found on " + gameObject.name);
        }

        punchTimer = new CooldownTimer(punchCooldown);
        kickTimer = new CooldownTimer(kickCooldown);
        jumpTimer = new CooldownTimer(jumpCooldown);
        specialTimer = new CooldownTimer(specialAttackCooldown);
        climbTimer = new CooldownTimer(climbCooldown); // Khởi tạo timer leo trèo

        Debug.Log("PlayerAttack initialized with cooldowns: " +
            $"Punch: {punchCooldown}s, Kick: {kickCooldown}s, Jump: {jumpCooldown}s, Special: {specialAttackCooldown}s, Climb: {climbCooldown}s");
    }


    public void Punch()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!specialTimer.IsReady) return; // Prevent punch during special cooldown
        if (!punchTimer.IsReady) return;
        HitType hitType = (HitType)(animationManager?.PlayHeadPunch());
        punchTimer.Trigger();

        hitReceiver.DelayedHit(hitType, punchDelay, attackRange, enemyLayer);
    }






    public void HoldPunch()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!specialTimer.IsReady) return;
        HitType hitType = (HitType)(animationManager?.PlayHoldPunch());
        specialTimer.Trigger();

        hitReceiver.DelayedHit(hitType, holdPunchDelay, attackRange, enemyLayer);
    }

    public void Kick()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!kickTimer.IsReady) return;
        if (!specialTimer.IsReady) return;
        HitType hitType = (HitType)(animationManager?.PlayKick());
        kickTimer.Trigger();

        hitReceiver.DelayedHit(hitType, kickDelay, attackRange, enemyLayer);
    }

    public void HoldKick()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!specialTimer.IsReady) return;
        HitType hitType = (HitType)(animationManager?.PlayHoldKick());
        playerMovement?.Jump();
        specialTimer.Trigger();

        hitReceiver.DelayedHit(hitType, holdKickDelay, attackRange, enemyLayer);
    }

    public void Jump()
    {
        if (!climbTimer.IsReady) return;
        if (!jumpTimer.IsReady) return;
        if (!specialTimer.IsReady) return;
        animationManager?.PlayJump();
        playerMovement?.Jump(); // Gọi vật lý nhảy từ PlayerMovement
        jumpTimer.Trigger();
    }

    public void JumpOverIntro()
    {
        // Giảm thời gian cooldown cho nhảy qua intro
        if (!climbTimer.IsReady) return;
        if (!specialTimer.IsReady) return;

        animationManager?.PlayJumpOverIntro();
        playerMovement?.JumpOverIntro(); // Gọi nhảy để vượt chướng ngại
        climbTimer.Trigger();
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
