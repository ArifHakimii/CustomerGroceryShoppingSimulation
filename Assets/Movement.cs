using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform playerCamera; // Reference to the player camera object
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f; // Smoothness of mouse movement
    [SerializeField] bool cursorLock = true; // Whether to lock the cursor
    [SerializeField] float mouseSensitivity = 3.5f; // Mouse sensitivity for rotation
    [SerializeField] float Speed = 6.0f; // Movement speed
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f; // Smoothness of movement
    [SerializeField] float gravity = -30f; // Gravity applied to the player
    [SerializeField] Transform groundCheck; // Transform of the ground check point
    [SerializeField] LayerMask ground; // Layer mask for detecting ground

    // Public variables
    public float jumpHeight = 6f; // Jump height
    float velocityY; // Vertical velocity
    bool isGrounded; // Whether the player is grounded

    // Private variables
    float cameraCap; // Rotation cap for the camera
    Vector2 currentMouseDelta; // Current mouse movement delta
    Vector2 currentMouseDeltaVelocity; // Velocity of mouse movement delta
    CharacterController controller; // Reference to the CharacterController component
    Vector2 currentDir; // Current movement direction
    Vector2 currentDirVelocity; // Velocity of movement direction
    Vector3 velocity; // Overall movement velocity

    void Start()
    {
        // Initialize variables
        controller = GetComponent<CharacterController>();

        // Lock the cursor if enabled
        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        UpdateMouse(); // Update mouse input and rotation
        UpdateMove(); // Update movement based on input and gravity
    }

    void UpdateMouse()
    {
        // Get mouse input
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Smooth out mouse movement
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        // Adjust camera rotation
        cameraCap -= currentMouseDelta.y * mouseSensitivity;
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f); // Clamp camera rotation to prevent flipping
        playerCamera.localEulerAngles = Vector3.right * cameraCap; // Apply camera rotation

        // Rotate player model
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMove()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        // Get movement input
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        // Smooth out movement direction
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        // Apply gravity
        velocityY += gravity * 2f * Time.deltaTime;

        // Calculate overall movement velocity
        velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;

        // Move the player
        controller.Move(velocity * Time.deltaTime);

        // Handle jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculate jump velocity
        }

        // Apply fall speed limit
        if(!isGrounded && controller.velocity.y < -1f)
        {
            velocityY = -8f; // Set fall speed limit
        }
    }
}
