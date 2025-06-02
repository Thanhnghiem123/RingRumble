
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputHandler : MonoBehaviour, IMovementInput
{
    public Vector2 touchStartPos;
    public float smoothTime = 0.1f;
    public Vector3 smoothedMoveDir = Vector3.zero;
    public bool isMoving = false;
    public Transform cameraTransform; // Kéo thả camera vào đây nếu muốn hướng theo camera

    public void ProcessInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isMoving = true;
            }

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                Vector2 touchCurrentPos = touch.position;
                Vector2 direction = touchCurrentPos - touchStartPos;

                if (direction.magnitude > 5f)
                {
                    Vector3 moveDir = new Vector3(direction.x, 0, direction.y).normalized;

                    // Nếu muốn hướng theo camera:
                    if (cameraTransform != null)
                    {
                        Vector3 camForward = cameraTransform.forward;
                        camForward.y = 0;
                        camForward.Normalize();
                        Vector3 camRight = cameraTransform.right;
                        camRight.y = 0;
                        camRight.Normalize();

                        moveDir = (camForward * direction.y + camRight * direction.x).normalized;
                    }

                    smoothedMoveDir = Vector3.Lerp(smoothedMoveDir, moveDir, smoothTime);
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                isMoving = false;
                smoothedMoveDir = Vector3.zero;
            }
        }
    }

    public bool IsMoving() => isMoving;
    public Vector3 GetDirection() => smoothedMoveDir;
}
