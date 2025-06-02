using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour, IPlayerMovement
{
    [Header("Movement Settings")]
    public float speed;
    public float rotationSpeed;
    public float jumpForce; // Lực nhảy, điều chỉnh trong Inspector
    public float groundCheckDistance = 0.2f; // Khoảng cách kiểm tra chạm đất
    public LayerMask groundLayer; // Layer để xác định mặt đất
    public bool isGrounded; // Trạng thái chạm đất
    public float jumpMoveDuration; // Thời gian di chuyển trước khi nhảy
    public float climbMoveDuration; // Thời gian di chuyển trước khi trèo

    [Header("Climb Settings")]
    public float climbForwardSpeedMultiplier = 0.4f; // Hệ số tốc độ khi tiếp cận vật cản
    public float climbHeight = 1.5f; // Chiều cao trèo
    public float climbDistance = 1f; // Khoảng cách tiến về phía trước khi trèo
    public float climbDuration = 1f; // Thời gian để trèo

    private IAnimationManager animationManager;
    private Rigidbody rb;
    private PlayerAttack playerAttack;
    private IMovementInput movementInput;

    public bool IsGrounded() => isGrounded; // Thuộc tính để kiểm tra trạng thái chạm đất

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on " + gameObject.name);
            enabled = false; // Tắt script nếu không có Rigidbody
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

        // Đảm bảo Rigidbody không bị khóa chuyển động
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
    }

    void Update()
    {
        // Kiểm tra xem player có chạm đất không
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    public void Move()
    {
        if (playerAttack != null && playerAttack.IsBlocking())
        {
            // Prevent movement
            return;
        }
        if (rb != null && movementInput != null)
        {
            Vector3 direction = movementInput.GetDirection();
            // Áp dụng vận tốc trên mặt phẳng XZ, giữ nguyên trọng lực
            Vector3 moveVelocity = direction.normalized * speed;
            moveVelocity.y = rb.linearVelocity.y; // Giữ vận tốc Y từ trọng lực
            rb.linearVelocity = new Vector3(moveVelocity.x, moveVelocity.y, moveVelocity.z);

            // Xoay nhân vật theo hướng di chuyển
            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    public void Stop()
    {
        if (rb != null)
        {
            // Đặt vận tốc về 0 trên XZ, giữ Y cho trọng lực
            //rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
        animationManager?.PlayRun(false);
        animationManager?.PlayIdle();
    }

    public void Jump()
    {
        if (rb != null && isGrounded)
        {
            // Bắt đầu coroutine nhảy
            StartCoroutine(JumpWithForwardDelay());
        }
    }

    private IEnumerator JumpWithForwardDelay()
    {
        // Chạy về phía trước trong jumpMoveDuration (chỉ XZ, không đụng tới Y)
        float timer = 0f;
        Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        float forwardSpeed = speed;
        Vector3 move;
        while (timer < jumpMoveDuration)
        {
            move = forwardXZ * forwardSpeed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z); // Giữ nguyên Y
            timer += Time.deltaTime;
            yield return null;
        }

        move = forwardXZ * forwardSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z); // Giữ nguyên Y

        // Tính hướng nhảy 45 độ giữa lên và trước (trên mặt phẳng XZ)
        Vector3 jumpDirection = (Vector3.up + transform.forward).normalized;
        Debug.Log("Jump Direction: " + jumpDirection);

        // Thêm lực nhảy theo hướng 45 độ
        rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
    }

    public void JumpOverIntro()
    {
        if (rb != null && isGrounded)
        {
            // Bắt đầu coroutine nhảy qua intro
            StartCoroutine(ClimbOverObstacleCoroutine());
        }
    }

    private IEnumerator ClimbOverObstacleCoroutine()
    {
        // Chạy về phía trước trong climbMoveDuration (chỉ XZ, không đụng tới Y)
        float timer = 0f;
        Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        float forwardSpeed = speed * climbForwardSpeedMultiplier; // Sử dụng tốc độ điều chỉnh
        Vector3 move;
        while (timer < climbMoveDuration)
        {
            move = forwardXZ * forwardSpeed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z); // Giữ nguyên Y
            timer += Time.deltaTime;
            yield return null;
        }
        move = forwardXZ * forwardSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z); // Giữ nguyên Y

        // Tính hướng 45 độ giữa lên và trước (trên mặt phẳng XZ)
        Vector3 climbDirection = (Vector3.up + transform.forward).normalized;
        // Điểm đích sau khi trèo (chiều cao và khoảng cách tùy chỉnh)
        Vector3 startPos = transform.position;
        Vector3 climbTarget = transform.position + climbDirection * climbDistance + Vector3.up * climbHeight;

        // Di chuyển lên từ từ theo hướng 45 độ trong thời gian climbDuration
        timer = 0f;

        while (timer < climbDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / climbDuration;
            transform.position = Vector3.Lerp(startPos, climbTarget, progress);
            yield return null;
        }

        // Đặt vị trí cuối cùng để tránh lỗi nhỏ
        transform.position = climbTarget;

        // Tiếp tục di chuyển về phía trước sau khi trèo
        rb.linearVelocity = new Vector3(forwardXZ.x * forwardSpeed, rb.linearVelocity.y, forwardXZ.z * forwardSpeed);
    }




}