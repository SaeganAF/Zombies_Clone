using UnityEngine;

/// <summary>
/// FirstPersonController handles the camera look (pitch/yaw) for a first-person perspective.
/// It rotates the player body left/right (yaw) and the camera up/down (pitch).
/// This script also manages cursor locking for mouse look gameplay.
/// </summary>
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    /// <summary>The entire player body - rotates left/right (horizontal, yaw rotation)</summary>
    public Transform playerBody;
    
    /// <summary>The camera component - rotates up/down (vertical, pitch rotation)</summary>
    public Camera playerCamera;

    [Header("Mouse Settings")]
    /// <summary>Controls how fast the camera moves when you move the mouse. Higher = more sensitive</summary>
    public float mouseSensitivity = 100f;
    
    /// <summary>Maximum angle you can look up or down (prevents flipping the view)</summary>
    public float maxPitch = 85f;

    /// <summary>Tracks current vertical rotation (pitch) to prevent exceeding maxPitch</summary>
    private float xRotation = 0f;

    void Start()
    {
        // If playerBody wasn't assigned in the Inspector, use this object's transform
        if (playerBody == null)
            playerBody = transform;
        
        // If playerCamera wasn't assigned, search for a Camera in the children of this object
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
        
        // Lock the cursor to the center of the screen and hide it (typical FPS behavior)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse movement input
        // Mouse X is horizontal (left/right), Mouse Y is vertical (up/down)
        // Multiply by sensitivity and deltaTime for frame-rate independent movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Subtract mouseY because moving mouse UP should look UP (negative pitch rotation)
        xRotation -= mouseY;
        
        // Clamp the rotation to prevent looking too far up or down
        // Mathf.Clamp keeps the value between -maxPitch and +maxPitch
        xRotation = Mathf.Clamp(xRotation, -maxPitch, maxPitch);

        // Apply vertical rotation (pitch) to the camera
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply horizontal rotation (yaw) to the entire player body
        // This makes the player turn left/right while looking around
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);

        // Handle ESC key to toggle cursor lock for testing/debugging
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // Unlock cursor if it's locked
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Lock cursor if it's unlocked
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
