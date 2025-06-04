using System.Collections;
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


    //[Header("Attack Settings")]
    //public float capsuleHeight = 2f;
    //public float capsuleRadius = 0.35f;
    




    //[Header("Attack Delay Settings")]
    //public float punchDelay = 0.2f;
    //public float kickDelay = 0.25f;
    //public float holdPunchDelay = 0.9f;
    //public float holdKickDelay = 0.35f;

    //public float damePunch = 10f;
    //public float dameKick = 15f;
    //public float dameHoldPunch = 20f;
    //public float dameHoldKick = 25f;
    //public float kickRange = 0.65f;


    //public LayerMask enemyLayer;
    //public LayerMask ropesLayer;

    private CooldownTimer punchTimer;
    private CooldownTimer kickTimer;
    private CooldownTimer jumpTimer;
    private CooldownTimer specialTimer;
    private CooldownTimer climbTimer;


    private IAnimationManager animationManager;
    private IPlayerAttack playerAttack;
    private IPlayerMovement playerMovement;
    private IMovementInput movementInput;
    //private PlayerHitReceiver hitReceiver;
    private bool isInitialized = false;

    // getters , setter
    //public float PunchDelay { get => punchDelay; set => punchDelay = value; }
    //public float KickDelay { get => kickDelay; set => kickDelay = value; }
    //public float HoldPunchDelay { get => holdPunchDelay; set => holdPunchDelay = value; }
    //public float HoldKickDelay { get => holdKickDelay; set => holdKickDelay = value; }
    //public float CapsuleHeight { get => capsuleHeight; set => capsuleHeight = value; }
    //public float CapsuleRadius { get => capsuleRadius; set => capsuleRadius = value; }

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
        //hitReceiver = GetComponent<PlayerHitReceiver>();
        //if (hitReceiver == null)
        //{
        //    Debug.LogError("HitReceiver component not found on " + gameObject.name);
        //}

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
