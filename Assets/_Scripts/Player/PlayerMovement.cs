using UnityEngine;
using UnityEngine.InputSystem; // เพิ่มตัวนี้เข้ามา

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 0.5f; // ระบบใหม่ค่าจะละเอียดกว่าเดิม ลองเริ่มที่น้อยๆ
    public Transform playerCamera;

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;
    private float gravity = -9.81f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. ระบบหมุนมุมกล้อง (ใช้ Mouse.current)
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

            xRotation -= mouseDelta.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseDelta.x);
        }

        // 2. ระบบเดิน (ใช้ Keyboard.current)
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            float x = 0;
            float z = 0;

            if (keyboard.wKey.isPressed) z = 1;
            if (keyboard.sKey.isPressed) z = -1;
            if (keyboard.aKey.isPressed) x = -1;
            if (keyboard.dKey.isPressed) x = 1;

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        // 3. แรงดึงดูด
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}