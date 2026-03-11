using UnityEngine;

/// <summary>
/// FirstPersonMovement handles player movement and gravity using Unity's CharacterController.
/// The CharacterController is ideal for FPS games because it provides:
/// - Built-in collision handling without physics
/// - Smooth movement without rigidbody quirks
/// - Precise control over gravity and jumping foundations
/// 
/// This script requires a CharacterController component on the same GameObject.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    /// <summary>How fast the player moves while walking normally (units per second)</summary>
    public float walkSpeed = 5f;
    
    /// <summary>How fast the player moves while running (holding Shift)</summary>
    public float runSpeed = 8f;

    [Header("Jump Settings")]
    /// <summary>How high the player jumps (higher = higher jump)</summary>
    public float jumpForce = 5f;

    /// <summary>Time in seconds before the player can jump again (prevents jump spam)</summary>
    public float jumpCooldown = 0.1f;

    [Header("Gravity Settings")]
    /// <summary>The force of gravity pulling the player down (negative value means down)</summary>
    public float gravity = -9.81f;

    /// <summary>How much faster the player falls compared to rising
    /// (higher = faster fall, lower = floaty fall)</summary>
    public float fallMultiplier = 2.5f;
    
    /// <summary>Transform position to check if player is on ground (usually at player's feet)</summary>
    public Transform groundCheck;
    
    /// <summary>Radius of the sphere used to check if player is touching the ground</summary>
    public float groundDistance = 0.2f;
    
    /// <summary>Layer mask to determine what counts as "ground" for collision detection</summary>
    public LayerMask groundMask = ~0; // ~0 means "everything" by default

    /// <summary>Reference to the CharacterController component - handles movement and collision</summary>
    private CharacterController controller;
    
    /// <summary>Stores the player's current velocity, especially for vertical (Y) movement)</summary>
    private Vector3 velocity;
    
    /// <summary>Tracks time since last jump to enforce cooldown</summary>
    private float jumpCooldownTimer;

    /// <summary>Is the player currently touching the ground?</summary>
    private bool isGrounded;

    void Start()
    {
        // Get the CharacterController component from this GameObject
        controller = GetComponent<CharacterController>();
        
        // If groundCheck wasn't assigned in Inspector, use this object's position
        if (groundCheck == null)
            groundCheck = transform;
    }

    void Update()
    {
        // Update ground state using a sphere check at the player's feet
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // If player is grounded and falling, keep player stuck to the ground
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Update the jump cooldown timer (never below 0)
        jumpCooldownTimer -= Time.deltaTime;

        // Jump input: Spacebar
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpCooldownTimer <= 0f)
        {
            // Calculate jump velocity using kinematic equation
            // velocity^2 = 2 * jumpHeight * -gravity
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

            // Reset cooldown so player can't spam jump
            jumpCooldownTimer = jumpCooldown;
        }

        // Get input for movement (WASD or Arrow keys)
        // Horizontal = A/D or Left/Right Arrow keys
        // Vertical = W/S or Up/Down Arrow keys
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction relative to where the player is looking
        // transform.right is the forward-relative LEFT/RIGHT direction
        // transform.forward is the camera's FORWARD direction
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Determine speed: run if holding Shift, otherwise walk
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        // Move the player horizontally (left/right and forward/backward)
        // Time.deltaTime makes movement frame-rate independent
        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity to velocity (pull downward each frame)
        velocity.y += gravity * Time.deltaTime;

        // If we are falling, apply extra gravity to make the fall feel less floaty
        // (this is a common FPS trick for more responsive jumping)
        if (velocity.y < 0f)
            velocity.y += gravity * (fallMultiplier - 1f) * Time.deltaTime;
        
        // Move the player vertically (mostly for gravity/falling)
        controller.Move(velocity * Time.deltaTime);
    }
}
