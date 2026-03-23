using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;

    [Header("Camera")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float maxLookAngle = 90f;

    [Header("Gravity & Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDrag = -2f;

    // Private fields
    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded;

    private void Start()
    {
        InitializeComponents();
        LockCursor();
    }

    private void Update()
    {
        UpdateGroundState();
        HandleCameraLook();
        HandleMovementInput();
        ApplyGravity();
        ApplyMovement();
    }

    /// <summary>
    /// Initialize required components
    /// </summary>
    private void InitializeComponents()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
            Debug.LogError("CharacterController not found on this GameObject!");

        if (playerCamera == null)
            Debug.LogWarning("Player Camera not assigned! Camera look will be disabled.");
    }

    /// <summary>
    /// Lock and hide the cursor
    /// </summary>
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Update ground detection
    /// </summary>
    private void UpdateGroundState()
    {
        isGrounded = controller.isGrounded;

        // Reset vertical velocity when grounded
        if (isGrounded && velocity.y < 0)
            velocity.y = groundDrag;
    }

    /// <summary>
    /// Handle camera rotation with mouse input
    /// </summary>
    private void HandleCameraLook()
    {
        if (playerCamera == null || Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        // Pitch (vertical look)
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yaw (horizontal look)
        transform.Rotate(Vector3.up * mouseDelta.x);
    }

    /// <summary>
    /// Handle keyboard input for movement and jumping
    /// </summary>
    private void HandleMovementInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        // Get movement input (WASD)
        float x = 0;
        float z = 0;

        if (keyboard.wKey.isPressed) z = 1;
        if (keyboard.sKey.isPressed) z = -1;
        if (keyboard.aKey.isPressed) x = -1;
        if (keyboard.dKey.isPressed) x = 1;

        // Check sprint input (Left Shift)
        float currentSpeed = keyboard.leftShiftKey.isPressed ? sprintSpeed : moveSpeed;

        // Calculate movement direction relative to player
        Vector3 moveDirection = (transform.right * x + transform.forward * z).normalized;
        Vector3 horizontalVelocity = moveDirection * currentSpeed;

        // Keep vertical velocity
        horizontalVelocity.y = velocity.y;
        velocity = horizontalVelocity;

        // Handle jump (Space)
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
            Jump();
    }

    /// <summary>
    /// Apply jump force
    /// </summary>
    private void Jump()
    {
        velocity.y = jumpForce;
        isGrounded = false;
    }

    /// <summary>
    /// Apply gravity to vertical velocity
    /// </summary>
    private void ApplyGravity()
    {
        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
    }

    /// <summary>
    /// Apply movement to character controller
    /// </summary>
    private void ApplyMovement()
    {
        if (controller != null)
            controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Unlock cursor (useful for pause menus)
    /// </summary>
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}