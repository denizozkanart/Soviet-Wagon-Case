using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleFPSController : MonoBehaviour
{
    public float walkSpeed = 0.01f;
    public float lookSensitivity = 0.005f; // Yeni sistemde fare hassasiyeti genelde daha düşük tutulmalı
    public Transform playerCamera;
    
    private CharacterController characterController;
    private float xRotation = 0f;
    private Vector3 velocity;
    private float gravity = -9.81f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // Oyun başladığında fare imlecini gizle ve ekrana kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Etrafa Bakma (Fare ile) - Yeni Input System
        float mouseX = 0f;
        float mouseY = 0f;
        
        if (Mouse.current != null)
        {
            mouseX = Mouse.current.delta.x.ReadValue() * lookSensitivity;
            mouseY = Mouse.current.delta.y.ReadValue() * lookSensitivity;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Kafayı tam aşağı/yukarı çevirmeyi sınırlar

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 2. Yürüme (W-A-S-D tuşları ile) - Yeni Input System
        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.wKey.isPressed) z += 1f;
            if (Keyboard.current.sKey.isPressed) z -= 1f;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        
        // Çapraz yürürken hızlanmayı önlemek için yön vektörünü normalize et
        if (move.magnitude > 1f) move.Normalize();
        
        characterController.Move(move * walkSpeed * Time.deltaTime);

        // 3. Yerçekimi (Havada kalmamak için)
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
