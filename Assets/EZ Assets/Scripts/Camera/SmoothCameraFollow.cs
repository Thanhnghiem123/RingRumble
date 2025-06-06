using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Tooltip("Transform của player để camera theo dõi")]
    public Transform player;
    [Tooltip("Tốc độ xoay mượt của camera, giá trị nhỏ = trễ nhiều hơn")]
    public float rotationSmoothSpeed = 5f; 
    [Tooltip("Khoảng cách offset giữa camera và player")]
    public Vector3 offset = new Vector3(0, 2f, -5f); 
    [Tooltip("Tốc độ di chuyển mượt của vị trí camera, giá trị nhỏ = trễ nhiều hơn")]
    public float positionSmoothSpeed = 5f; 

    private void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned!");
            return;
        }

        Vector3 desiredPosition = player.position + player.TransformDirection(offset);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        Quaternion desiredRotation = Quaternion.LookRotation(player.position - transform.position, Vector3.up);

        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);

        transform.rotation = smoothedRotation;
    }
}