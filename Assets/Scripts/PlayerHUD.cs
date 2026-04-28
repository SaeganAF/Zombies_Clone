using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// PlayerHUD wires up the Unity UI canvas to PlayerStats data.
/// Every time a stat changes, PlayerStats fires an event and
/// PlayerHUD updates the matching UI element.
///
/// ═══════════════════════════════════════════
/// HOW TO SET UP THE HUD CANVAS
/// ═══════════════════════════════════════════
///
/// 1. CREATE THE CANVAS
///    • GameObject → UI → Canvas
///    • Set Canvas "Render Mode" to Screen Space - Overlay
///    • On the Canvas Scaler component:
///        UI Scale Mode  → Scale With Screen Size
///        Reference Res  → 1920 × 1080
///        Match          → 0.5
///
/// 2. ADD A CANVAS GROUP (optional, for fade-in on start)
///    • Add a CanvasGroup component to the Canvas itself.
///
/// 3. CREATE EACH HUD ELEMENT (all inside the Canvas)
///    ┌─────────────────────────────────────────────────────────┐
///    │ ELEMENT          │ TYPE              │ ANCHOR           │
///    ├─────────────────────────────────────────────────────────┤
///    │ HealthText       │ TextMeshPro - UI  │ Bottom-Left      │
///    │ HealthBar        │ UI Slider         │ Bottom-Left      │
///    │ PointsText       │ TextMeshPro - UI  │ Top-Center       │
///    │ RoundText        │ TextMeshPro - UI  │ Top-Left         │
///    │ AmmoText         │ TextMeshPro - UI  │ Bottom-Right     │
///    │ CrosshairImage   │ UI Image          │ Center           │
///    │ HitMarkerImage   │ UI Image (red ✕)  │ Center           │
///    │ BuyPromptPanel   │ Panel             │ Center           │
///    │  └ BuyPromptText │ TextMeshPro - UI  │ fill parent      │
///    └─────────────────────────────────────────────────────────┘
///
/// 4. ATTACH THIS SCRIPT to the Canvas (or an empty HUD GameObject).
///
/// 5. DRAG each UI object into the matching slot in the Inspector.
///
/// ═══════════════════════════════════════════
/// SUGGESTED TEXT FORMATS (editable here)
/// ═══════════════════════════════════════════
///   Health  → "100 HP"
///   Points  → "500 PTS"
///   Round   → "ROUND 1"
///   Ammo    → "30 / 120"
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    // ──────────────────────────────────────────
    //  Inspector References
    // ──────────────────────────────────────────

    [Header("Health UI")]
    [Tooltip("TextMeshPro text that shows current HP")]
    public TextMeshProUGUI healthText;

    [Tooltip("Optional Slider used as a health bar (set Max Value to player maxHealth)")]
    public Slider healthBar;

    [Header("Points UI")]
    [Tooltip("TextMeshPro text showing current points")]
    public TextMeshProUGUI pointsText;

    [Header("Round UI")]
    [Tooltip("TextMeshPro text showing current round")]
    public TextMeshProUGUI roundText;

    [Header("Ammo UI")]
    [Tooltip("TextMeshPro text showing ammo — format: 'mag / reserve'")]
    public TextMeshProUGUI ammoText;

    [Header("Crosshair")]
    [Tooltip("Center-screen crosshair image")]
    public Image crosshairImage;

    [Header("Hit Marker")]
    [Tooltip("Red hit-marker image — shown briefly when player hits an enemy")]
    public Image hitMarkerImage;

    [Tooltip("How long (seconds) the hit marker stays visible")]
    public float hitMarkerDuration = 0.1f;

    [Header("Buy Prompt")]
    [Tooltip("Panel that appears when the player is near a buyable door")]
    public GameObject buyPromptPanel;

    [Tooltip("Text inside the buy prompt panel")]
    public TextMeshProUGUI buyPromptText;

    [Header("Low Health Flash")]
    [Tooltip("Full-screen red image used to flash when health is low")]
    public Image lowHealthVignette;

    [Tooltip("Health % below which the low-health vignette pulses")]
    [Range(0f, 1f)]
    public float lowHealthThreshold = 0.35f;

    // ──────────────────────────────────────────
    //  Private State
    // ──────────────────────────────────────────
    private float hitMarkerTimer;
    private bool  vignetteActive;

    // ──────────────────────────────────────────
    //  Lifecycle
    // ──────────────────────────────────────────
    void Start()
    {
        // Wait one frame so PlayerStats.Instance is guaranteed to exist
        if (PlayerStats.Instance == null)
        {
            Debug.LogError("PlayerHUD: No PlayerStats found in scene. " +
                           "Make sure PlayerStats is attached to the Player.");
            return;
        }

        // Subscribe to stat change events
        PlayerStats.Instance.OnHealthChanged.AddListener(UpdateHealthUI);
        PlayerStats.Instance.OnPointsChanged.AddListener(UpdatePointsUI);
        PlayerStats.Instance.OnAmmoChanged  .AddListener(UpdateAmmoUI);
        PlayerStats.Instance.OnRoundChanged .AddListener(UpdateRoundUI);

        // Initial update so the HUD isn't blank at game start
        UpdateHealthUI();
        UpdatePointsUI();
        UpdateAmmoUI();
        UpdateRoundUI();

        // Hide hit marker and buy prompt on start
        if (hitMarkerImage != null) hitMarkerImage.gameObject.SetActive(false);
        if (buyPromptPanel  != null) buyPromptPanel.SetActive(false);
        if (lowHealthVignette != null)
        {
            Color c = lowHealthVignette.color;
            c.a = 0f;
            lowHealthVignette.color = c;
        }
    }

    void Update()
    {
        // Hit marker timeout
        if (hitMarkerTimer > 0f)
        {
            hitMarkerTimer -= Time.deltaTime;
            if (hitMarkerTimer <= 0f && hitMarkerImage != null)
                hitMarkerImage.gameObject.SetActive(false);
        }

        // Low-health vignette pulse
        if (lowHealthVignette != null && PlayerStats.Instance != null)
        {
            float hp     = (float)PlayerStats.Instance.CurrentHealth / PlayerStats.Instance.maxHealth;
            bool  isLow  = hp < lowHealthThreshold && !PlayerStats.Instance.IsDead;

            if (isLow)
            {
                // Pulse alpha between 0.15 and 0.5
                float pulse = 0.3f + Mathf.Sin(Time.time * 3f) * 0.2f;
                Color c = lowHealthVignette.color;
                c.a = pulse;
                lowHealthVignette.color = c;
            }
            else
            {
                // Fade out
                Color c = lowHealthVignette.color;
                c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * 2f);
                lowHealthVignette.color = c;
            }
        }
    }

    // ──────────────────────────────────────────
    //  UI Update Methods (called by events)
    // ──────────────────────────────────────────

    void UpdateHealthUI()
    {
        if (PlayerStats.Instance == null) return;

        int hp = PlayerStats.Instance.CurrentHealth;

        if (healthText != null)
            healthText.text = hp + " HP";

        if (healthBar != null)
        {
            healthBar.maxValue = PlayerStats.Instance.maxHealth;
            healthBar.value    = hp;
        }
    }

    void UpdatePointsUI()
    {
        if (PlayerStats.Instance == null || pointsText == null) return;
        pointsText.text = PlayerStats.Instance.Points.ToString("N0") + " PTS";
    }

    void UpdateAmmoUI()
    {
        if (PlayerStats.Instance == null || ammoText == null) return;
        ammoText.text = PlayerStats.Instance.AmmoInMag + " / " + PlayerStats.Instance.AmmoReserve;
    }

    void UpdateRoundUI()
    {
        if (PlayerStats.Instance == null || roundText == null) return;
        roundText.text = "ROUND " + PlayerStats.Instance.CurrentRound;
    }

    // ──────────────────────────────────────────
    //  Public Helpers (called by other scripts)
    // ──────────────────────────────────────────

    /// <summary>
    /// Flash the hit marker for hitMarkerDuration seconds.
    /// Call this from PlayerShoot when a bullet hits an enemy.
    /// </summary>
    public void ShowHitMarker()
    {
        if (hitMarkerImage == null) return;
        hitMarkerImage.gameObject.SetActive(true);
        hitMarkerTimer = hitMarkerDuration;
    }

    /// <summary>Show the buy prompt with a custom message.</summary>
    public void ShowBuyPrompt(string message)
    {
        if (buyPromptPanel == null) return;
        buyPromptPanel.SetActive(true);
        if (buyPromptText != null) buyPromptText.text = message;
    }

    /// <summary>Hide the buy prompt.</summary>
    public void HideBuyPrompt()
    {
        if (buyPromptPanel != null) buyPromptPanel.SetActive(false);
    }
}
