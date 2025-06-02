using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform player; // Kéo thả Transform của player vào đây trong Inspector
    public float rotationSmoothSpeed = 5f; // Tốc độ xoay mượt của camera, giá trị nhỏ = trễ nhiều hơn
    public Vector3 offset = new Vector3(0, 2f, -5f); // Offset giữa camera và player (có thể điều chỉnh)
    public float positionSmoothSpeed = 5f; // Tốc độ di chuyển mượt của vị trí camera

    private void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned!");
            return;
        }

        // Tính vị trí mong muốn của camera
        Vector3 desiredPosition = player.position + player.TransformDirection(offset);

        // Làm mượt vị trí camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);

        // Cập nhật vị trí camera
        transform.position = smoothedPosition;

        // Tính góc quay mong muốn (dựa trên hướng của player)
        Quaternion desiredRotation = Quaternion.LookRotation(player.position - transform.position, Vector3.up);

        // Làm mượt góc quay của camera
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);

        // Cập nhật góc quay camera
        transform.rotation = smoothedRotation;
    }
}