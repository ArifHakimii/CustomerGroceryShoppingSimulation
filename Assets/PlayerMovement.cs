using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovementTutorial : MonoBehaviour
{
    // Define variables related to movement
    [Header("Movement")]
    public float moveSpeed; // Base movement speed
    public float groundDrag; // Ground friction
    public float jumpForce; // Jump force
    public float jumpCooldown; // Time between jumps
    public float airMultiplier; // Movement speed multiplier in air
    bool readyToJump; // Flag indicating whether the player can jump

    // Define hidden variables to store different movement speeds
    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    // Define keybinds for actions
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space; // Key for jumping

    // Define variables for ground check
    [Header("Ground Check")]
    public float playerHeight; // Height of the player's feet
    public LayerMask whatIsGround; // Layer mask to identify the ground
    bool grounded; // Flag indicating whether the player is on the ground

    // Define reference variables for orientation and movement
    public Transform orientation; // Transform for player's orientation
    float horizontalInput; // Horizontal input from the player
    float verticalInput; // Vertical input from the player
    Vector3 moveDirection; // Direction of player's movement
    Rigidbody rb; // Rigidbody component of the player

    private void Start()
    {
        // Get the Rigidbody component of the player
        rb = GetComponent<Rigidbody>();

        // Prevent the player from rotating freely
        rb.freezeRotation = true;

        // Initialize the flag indicating the player can jump
        readyToJump = true;
    }

    // Update method is called once per frame
    private void Update()
    {
        // Check if the player is on the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        // Process player input
        MyInput();

        // Adjust movement speed based on input
        SpeedControl();

        // Apply ground drag or no drag depending on the player's state
        if (grounded)
        {
            rb.drag = groundDrag; // Apply ground drag
        }
        else
        {
            rb.drag = 0; // Remove drag when in the air
        }
    }

    // FixedUpdate method is called once per physics update
    private void FixedUpdate()
    {
        // Move the player based on the calculated movement direction
        MovePlayer();
    }

    // Handle player input for movement and jumping
    private void MyInput()
    {
        // Get horizontal and vertical input values
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Check if the jump key is pressed and the player is on the ground and ready to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            // Set the flag indicating the player is not ready to jump
            readyToJump = false;

            // Make the player jump
            Jump();

            // Call the ResetJump method after the jump cooldown period
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}
