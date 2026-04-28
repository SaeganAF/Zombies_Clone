using UnityEngine;

/// <summary>
/// BuyableDoor lets the player spend points to open a blocked-off area,
/// just like Call of Duty: Zombies.
///
/// ═══════════════════════════════════════════
/// HOW TO SET UP A BUYABLE DOOR
/// ═══════════════════════════════════════════
///
/// 1. CREATE THE DOOR OBJECT
///    • Model a door (or use a cube as a placeholder).
///    • Add a BoxCollider to it — check "Is Trigger".
///    • Attach this BuyableDoor script to the same GameObject.
///
/// 2. ASSIGN THE DOOR COST
///    • In the Inspector, set "Cost" (e.g. 750).
///
/// 3. ASSIGN THE DOOR OBJECT TO OPEN
///    • "Door Object" → the GameObject to deactivate (hide) when opened.
///      This is usually the visible door mesh / barrier.
///    • "Door Blocker" → an optional second collider that physically
///      blocks movement (if your door mesh doesn't have one).
///
/// 4. OPTIONAL: FLOATING COST TEXT
///    • Add a World Space Canvas child with a TextMeshPro text component.
///    • Assign it to "Cost Text" — it will display "750 Points [F]".
///
/// 5. THE TRIGGER ZONE
///    • The trigger collider on this GameObject defines the zone where
///      the player sees the prompt. Make it slightly larger than the door.
///    • Make sure your Player has the tag "Player".
///
/// 6. LINK THE HUD
///    • Assign the PlayerHUD reference, OR leave it blank — the script
///      will find it automatically at runtime via FindObjectOfType.
/// </summary>
public class BuyableDoor : MonoBehaviour
{
    // ──────────────────────────────────────────
    //  Inspector Settings
    // ──────────────────────────────────────────

    [Header("Door Settings")]
    [Tooltip("How many points the player must spend to open this door.")]
    public int cost = 750;

    [Tooltip("Key the player must press to buy. Default: F")]
    public KeyCode buyKey = KeyCode.F;

    [Tooltip("The physical door / barrier GameObject to hide when opened.")]
    public GameObject doorObject;

    [Tooltip("Optional second collider that blocks movement (can be the same as doorObject).")]
    public Collider doorBlocker;

    [Header("UI")]
    [Tooltip("Optional: WorldSpace TextMeshPro above the door showing cost.")]
    public TMPro.TextMeshPro costLabel;

    [Tooltip("If left blank, the script finds PlayerHUD automatically.")]
    public PlayerHUD playerHUD;

    [Header("State")]
    [Tooltip("Is this door already open? Useful for debugging or pre-opened rooms.")]
    public bool isOpen = false;

    // ──────────────────────────────────────────
    //  Private State
    // ──────────────────────────────────────────
    private bool playerInRange = false;

    // ──────────────────────────────────────────
    //  Lifecycle
    // ──────────────────────────────────────────
    void Start()
    {
        // Auto-find HUD if not assigned
        if (playerHUD == null)
            playerHUD = FindObjectOfType<PlayerHUD>();

        // Set up the world-space cost label
        if (costLabel != null)
            costLabel.text = cost.ToString("N0") + " Points  [" + buyKey.ToString() + "]";

        // If the door starts open, immediately open it without spending points
        if (isOpen) Open(free: true);
    }

    void Update()
    {
        if (!playerInRange || isOpen) return;

        if (Input.GetKeyDown(buyKey))
            TryBuy();
    }

    // ──────────────────────────────────────────
    //  Trigger Detection
    // ──────────────────────────────────────────

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isOpen) return;

        playerInRange = true;

        // Show HUD prompt
        if (playerHUD != null)
        {
            int pts = PlayerStats.Instance != null ? PlayerStats.Instance.Points : 0;
            bool canAfford = pts >= cost;

            string msg = "<b>[" + buyKey.ToString() + "] Open Door</b>\n" +
                         "<color=" + (canAfford ? "#FFD700" : "#FF4444") + ">" +
                         cost.ToString("N0") + " Points</color>";

            if (!canAfford)
                msg += "\n<size=70%><color=#FF6666>Not enough points</color></size>";

            playerHUD.ShowBuyPrompt(msg);
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Refresh prompt in case points changed while standing in range
        if (!other.CompareTag("Player") || isOpen) return;

        if (playerHUD != null)
        {
            int  pts       = PlayerStats.Instance != null ? PlayerStats.Instance.Points : 0;
            bool canAfford = pts >= cost;

            string msg = "<b>[" + buyKey.ToString() + "] Open Door</b>\n" +
                         "<color=" + (canAfford ? "#FFD700" : "#FF4444") + ">" +
                         cost.ToString("N0") + " Points</color>";

            if (!canAfford)
                msg += "\n<size=70%><color=#FF6666>Not enough points</color></size>";

            playerHUD.ShowBuyPrompt(msg);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (playerHUD != null) playerHUD.HideBuyPrompt();
    }

    // ──────────────────────────────────────────
    //  Purchase Logic
    // ──────────────────────────────────────────

    void TryBuy()
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogWarning("BuyableDoor: PlayerStats not found in scene.");
            return;
        }

        bool success = PlayerStats.Instance.SpendPoints(cost);

        if (success)
        {
            Debug.Log("Door opened! Spent " + cost + " points.");
            Open(free: false);
        }
        else
        {
            Debug.Log("Not enough points to open door. Need " + cost +
                      ", have " + PlayerStats.Instance.Points);
            // Optional: play a "can't afford" sound here
        }
    }

    /// <summary>Open (remove) the door. Pass free:true to skip the point deduction.</summary>
    public void Open(bool free = false)
    {
        isOpen = true;
        playerInRange = false;

        // Hide the visual door
        if (doorObject != null) doorObject.SetActive(false);

        // Disable the physics blocker
        if (doorBlocker != null) doorBlocker.enabled = false;

        // Hide the cost label
        if (costLabel != null) costLabel.gameObject.SetActive(false);

        // Hide the buy prompt
        if (playerHUD != null) playerHUD.HideBuyPrompt();

        Debug.Log("Door opened" + (free ? " (free)" : "") + ".");
    }
}
