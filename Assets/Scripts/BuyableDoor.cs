using UnityEngine;

/// <summary>
/// BuyableDoor lets the player spend points to open a blocked-off area,
/// just like Call of Duty: Zombies.
/// </summary>
public class BuyableDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public int cost = 750;
    public KeyCode buyKey = KeyCode.F;

    [Tooltip("The physical door / barrier GameObject to hide when opened.")]
    public GameObject doorObject;

    [Tooltip("Optional second collider that blocks movement.")]
    public Collider doorBlocker;

    [Header("Spawn Points To Activate")]
    [Tooltip("Spawn points that should activate when this door opens.")]
    public GameObject[] spawnPointsToActivate;

    [Header("UI")]
    public TMPro.TextMeshPro costLabel;
    public PlayerHUD playerHUD;

    [Header("State")]
    public bool isOpen = false;

    // Private
    private bool playerInRange = false;
    private UnityEngine.AI.NavMeshObstacle navObstacle;

    void Start()
    {
        // Auto-find HUD
        if (playerHUD == null)
            playerHUD = FindObjectOfType<PlayerHUD>();

        // Cache NavMeshObstacle if present
        navObstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();

        // Set cost label
        if (costLabel != null)
            costLabel.text = cost.ToString("N0") + " Points  [" + buyKey.ToString() + "]";

        // If door starts open, open it immediately
        if (isOpen) Open(free: true);
    }

    void Update()
    {
        if (!playerInRange || isOpen) return;

        if (Input.GetKeyDown(buyKey))
            TryBuy();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isOpen) return;

        playerInRange = true;

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
        if (!other.CompareTag("Player") || isOpen) return;

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

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (playerHUD != null) playerHUD.HideBuyPrompt();
    }

    // Purchase Logic
    void TryBuy()
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogWarning("BuyableDoor: PlayerStats not found.");
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
            Debug.Log("Not enough points to open door.");
        }
    }

    public void Open(bool free = false)
    {
        isOpen = true;
        playerInRange = false;

        // Disable NavMeshObstacle so enemies can path through
        if (navObstacle != null)
        {
            navObstacle.carving = false;
            navObstacle.enabled = false;
        }

        // Hide door visuals
        if (doorObject != null) doorObject.SetActive(false);

        // Disable physical blocker
        if (doorBlocker != null) doorBlocker.enabled = false;

        // Hide cost label
        if (costLabel != null) costLabel.gameObject.SetActive(false);

        // Hide HUD prompt
        if (playerHUD != null) playerHUD.HideBuyPrompt();

        // Activate connected spawn points
        foreach (var sp in spawnPointsToActivate)
        {
            if (sp != null)
                sp.SetActive(true);
        }

        Debug.Log("Door opened" + (free ? " (free)" : "") + ".");
    }
}
