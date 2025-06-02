using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IMovementInput movementInput;
    private Player playerInteract;
    private bool isInitialized = false;

    void Start()
    {
        movementInput = GetComponent<IMovementInput>();
        playerInteract = GetComponent<Player>();

        if (movementInput == null)
        {
            Debug.LogError("TouchInputHandler component is missing on " + gameObject.name);
        }
        if (playerInteract == null)
        {
            Debug.LogError("PlayerMovement component is missing on " + gameObject.name);
        }

        isInitialized = (movementInput != null && playerInteract != null);
        if (!isInitialized)
        {
            Debug.LogError("PlayerController initialization failed. Disabling script.");
            enabled = false;
        }
    }

    void Update()
    {
        if (!isInitialized) return;

        movementInput.ProcessInput();
        if (movementInput.IsMoving())
        {
            playerInteract.Move();
        }
        else
        {
            playerInteract.Stop();
        }
    }
}