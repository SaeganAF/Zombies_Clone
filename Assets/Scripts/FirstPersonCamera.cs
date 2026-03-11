using UnityEngine;

/// <summary>
/// FirstPersonCameraSetup is a utility script that helps configure and manage the first-person camera.
/// While basic camera rotation is handled by FirstPersonController, this script provides utilities
/// for camera effects, field of view, and other camera-related settings.
/// 
/// You can expand this script to add features like:
/// - Camera bob (movement effect)
/// - Field of view changes (zoom/scope)
/// - Head tilt on strafe
/// - Recoil on weapon fire
/// </summary>
public class FirstPersonCameraSetup : MonoBehaviour
{
    [Header("Camera References")]
    /// <summary>The main camera rendering the world</summary>
    public Camera mainCamera;
    
    [Header("Camera Settings")]
    /// <summary>Default field of view (how wide the camera can see)</summary>
    public float defaultFOV = 60f;
    
    /// <summary>How much the FOV changes when looking through a scope/zooming</summary>
    public float zoomedFOV = 30f;
    
    /// <summary>Speed of transitioning between FOV values</summary>
    public float fovTransitionSpeed = 10f;

    /// <summary>Current target field of view that we're transitioning towards</summary>
    private float targetFOV;

    void Start()
    {
        // Get the Camera component from this GameObject or children
        if (mainCamera == null)
            mainCamera = GetComponentInChildren<Camera>();
        
        // Initialize FOV to default
        if (mainCamera != null)
        {
            targetFOV = defaultFOV;
            mainCamera.fieldOfView = defaultFOV;
        }
    }

    void Update()
    {
        // Smoothly transition camera FOV towards target FOV
        if (mainCamera != null)
        {
            // Lerp (Linear interpolation) smoothly blends current FOV to target FOV
            mainCamera.fieldOfView = Mathf.Lerp(
                mainCamera.fieldOfView,    // Current FOV
                targetFOV,                 // Target FOV
                fovTransitionSpeed * Time.deltaTime  // Speed (frame-rate independent)
            );
        }

        // Example: Press scroll wheel to zoom in/out
        // Scroll wheel provides mouse wheel input (positive up, negative down)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput > 0)
        {
            // Scroll up = zoom in
            targetFOV = zoomedFOV;
        }
        else if (scrollInput < 0)
        {
            // Scroll down = zoom out
            targetFOV = defaultFOV;
        }
    }

    /// <summary>
    /// Public method to set custom FOV (useful for scope/zoom effects)
    /// </summary>
    public void SetFOV(float newFOV)
    {
        targetFOV = newFOV;
    }

    /// <summary>
    /// Reset camera FOV to default
    /// </summary>
    public void ResetFOV()
    {
        targetFOV = defaultFOV;
    }
}