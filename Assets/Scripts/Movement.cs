using UnityEngine;

/// <summary>
/// PlayerJump handles jumping mechanics for the player.
/// This works in conjunction with FirstPersonMovement to add jumping capability.
/// The jump force is applied to the CharacterController's velocity.
/// 
/// IMPORTANT: Attach this to the same GameObject as FirstPersonMovement.
/// This script will retrieve and modify the FirstPersonMovement's velocity.
/// </summary>
public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    /// <summary>How much upward force to apply when jumping (higher = higher jump)</summary>
    public float jumpForce = 5f;
    
    /// <summary>Time in seconds before player can jump again (prevents jump spamming)</summary>
    public float jumpCooldown = 0.1f;

    [Header("Ground Detection")]
    /// <summary>Transform at the player's feet to detect if standing on ground</summary>
    public Transform groundCheck;
    
    /// <summary>Radius of sphere cast for ground detection</summary>
    public float groundDistance = 0.2f;
    
    /// <summary>What layers count as "ground" for jumping</summary>
    public LayerMask groundMask = ~0;

    /// <summary>Reference to CharacterController for jump velocity application</summary>
    private CharacterController controller;
    
    /// <summary>Tracks time since last jump to enforce cooldown</summary>
    private float jumpCooldownTimer = 0f;
    
    /// <summary>Is player currently touching the ground?</summary>
    private bool isGrounded;

    void Start()
    {
        // Get the CharacterController from this GameObject
        controller = GetComponent<CharacterController>();
        
        // If groundCheck wasn't assigned, use this object's transform
        if (groundCheck == null)
            groundCheck = transform;
    }

    void Update()
    {
        // Decrement jump cooldown timer each frame (but don't go below 0)
        jumpCooldownTimer -= Time.deltaTime;

        // Check if player is standing on ground using a sphere cast
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Listen for jump input (Spacebar)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Try to jump
            TryJump();
        }
    }

    /// <summary>
    /// Attempts to make the player jump if allowed (grounded and cooldown expired).
    /// Applies upward velocity to the CharacterController.
    /// </summary>
    private void TryJump()
    {
        // Check if player is grounded AND jump cooldown has expired
        if (isGrounded && jumpCooldownTimer <= 0f)
        {
            // Apply upward force by using CharacterController's MOVE function with velocity
            // Note: The vertical velocity will be handled by FirstPersonMovement's gravity
            Vector3 jumpVelocity = Vector3.up * jumpForce;
            controller.Move(jumpVelocity * Time.deltaTime);
            
            // Reset cooldown timer to prevent jump spamming
            jumpCooldownTimer = jumpCooldown;
        }
    }
}