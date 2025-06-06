using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour, IPlayerMovement
{
    [Header("Movement Settings")]
    [Tooltip("Tốc độ di chuyển của nhân vật")]
    public float speed;
    [Tooltip("Tốc độ xoay khi đổi hướng")]
    public float rotationSpeed;
    [Tooltip("Lực nhảy áp dụng theo hướng 45 độ")]
    public float jumpForce; 
    [Tooltip("Khoảng cách kiểm tra mặt đất phía dưới nhân vật")]
    public float groundCheckDistance = 0.2f; 
    [Tooltip("Layer được coi là mặt đất")]
    public LayerMask groundLayer; 
    [Tooltip("Trạng thái tiếp xúc với mặt đất")]
    public bool isGrounded;
    [Tooltip("Thời gian nhân vật di chuyển về phía trước trước khi nhảy")]
    public float jumpMoveDuration; 
    [Tooltip("Thời gian nhân vật di chuyển về phía trước trước khi trèo")]
    public float climbMoveDuration; 

    [Header("Climb Settings")]
    [Tooltip("Hệ số giảm tốc độ khi tiếp cận vật cản")]
    public float climbForwardSpeedMultiplier = 0.4f; 
    [Tooltip("Chiều cao tối đa khi trèo vật cản")]
    public float climbHeight = 1.5f; 
    [Tooltip("Khoảng cách tiến về phía trước khi hoàn thành trèo")]
    public float climbDistance = 1f; 
    [Tooltip("Thời gian thực hiện hoàn tất động tác trèo")]
    public float climbDuration = 1f; 


    [Header("LayerMask")]
    public LayerMask ropesLayer;
    private PlayerHitReceiver hitReceiver;


    private IAnimationManager animationManager;
    private Rigidbody rb;
    private PlayerAttack playerAttack;
    private IMovementInput movementInput;

    public float Speed { get => speed; set => speed = value; } 

    public bool IsGrounded() => isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer); 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on " + gameObject.name);
            enabled = false; 
            return;
        }
        animationManager = GetComponent<IAnimationManager>();
        if (animationManager == null)
        {
            Debug.LogError("AnimationManager not found on " + gameObject.name);
        }
        else
        {
            animationManager.PlayIdle();
        }

        rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);

        playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack == null)
        {
            Debug.LogError("PlayerAttack component not found on " + gameObject.name);
        }
        movementInput = GetComponent<IMovementInput>();
        if (movementInput == null)
        {
            Debug.LogError("MovementInput component not found on " + gameObject.name);
        }
        hitReceiver = GetComponent<PlayerHitReceiver>();
        if (hitReceiver == null)
        {
            Debug.LogError("HitReceiver component not found on " + gameObject.name);
        }
    }




    public bool CanClimb(float capsuleHeight, float capsuleRadius)
    {
        Collider[] hits = hitReceiver.DetectColliders(capsuleHeight, capsuleRadius, ropesLayer);
        Debug.Log($"CanClimb: Detected {hits.Length} hits on ropes layer.");
        return hits.Length > 0;
    }


    /// <summary>
    /// Di chuyển nhân vật dựa trên đầu vào từ người chơi.
    /// </summary>
    public void Move()
    {
        if (playerAttack != null && playerAttack.IsBlocking())
        {
            // Prevent movement
            return;
        }
        if (!AttackManager.PNormalState) return;
        if (rb != null && movementInput != null)
        {
            Vector3 direction = movementInput.GetDirection();
            Vector3 moveVelocity = direction.normalized * speed;
            moveVelocity.y = rb.linearVelocity.y; 
            rb.linearVelocity = new Vector3(moveVelocity.x, moveVelocity.y, moveVelocity.z);

            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }


    /// <summary>
    /// Dừng di chuyển của nhân vật, đặt vận tốc về 0 trên XZ.
    /// </summary>
    public void Stop()
    {
        if (rb != null)
        {
        }
        animationManager?.PlayRun(false);
        animationManager?.PlayIdle();
    }

    /// <summary>
    /// Thực hiện nhảy với một khoảng thời gian di chuyển về phía trước trước khi nhảy.
    /// Nhân vật sẽ chạy về phía trước trong một khoảng thời gian nhất định trước khi thực hiện nhảy.
    /// Hướng nhảy sẽ là 45 độ giữa hướng lên và hướng đi về phía trước.
    /// </summary>
    public void Jump()
    {
        if (rb != null && isGrounded)
        {
            // Bắt đầu coroutine nhảy
            StartCoroutine(JumpWithForwardDelay());
        }
    }

    /// <summary>
    /// Coroutine thực hiện nhảy với một khoảng thời gian di chuyển về phía trước.
    /// </summary>
    /// <returns></returns>
    private IEnumerator JumpWithForwardDelay()
    {
        float timer = 0f;
        Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        float forwardSpeed = speed;
        Vector3 move;
        while (timer < jumpMoveDuration)
        {
            move = forwardXZ * forwardSpeed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z); 
            timer += Time.deltaTime;
            yield return null;
        }

        move = forwardXZ * forwardSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z); 

        Vector3 jumpDirection = (Vector3.up + transform.forward).normalized;
        Debug.Log("Jump Direction: " + jumpDirection);

        // Thêm lực nhảy theo hướng 45 độ
        rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Thực hiện nhảy qua intro, nhân vật sẽ chạy về phía trước và trèo qua vật cản.
    /// </summary>
    public void JumpOverIntro()
    {
        if (rb != null && isGrounded)
        {
            StartCoroutine(ClimbOverObstacleCoroutine());
        }
    }

    /// <summary>
    /// Coroutine thực hiện trèo qua vật cản.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClimbOverObstacleCoroutine()
    {
        float timer = 0f;
        Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        float forwardSpeed = speed * climbForwardSpeedMultiplier; 
        Vector3 move;
        while (timer < climbMoveDuration)
        {
            move = forwardXZ * forwardSpeed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z); 
            timer += Time.deltaTime;
            yield return null;
        }
        move = forwardXZ * forwardSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z); 

        Vector3 climbDirection = (Vector3.up + transform.forward).normalized;
        Vector3 startPos = transform.position;
        Vector3 climbTarget = transform.position + climbDirection * climbDistance + Vector3.up * climbHeight;

        timer = 0f;

        while (timer < climbDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / climbDuration;
            transform.position = Vector3.Lerp(startPos, climbTarget, progress);
            yield return null;
        }

        transform.position = climbTarget;

        rb.linearVelocity = new Vector3(forwardXZ.x * forwardSpeed, rb.linearVelocity.y, forwardXZ.z * forwardSpeed);
    }
}
