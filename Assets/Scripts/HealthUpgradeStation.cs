using UnityEngine;

/// <summary>
/// HealthUpgradeStation lets the player buy a health upgrade to increase max health to 150 for points by pressing F near it.
///
/// HOW TO SET UP:
///   1. Create a cube in the Hierarchy (3D Object -> Cube), rename it "HealthUpgradeStation"
///   2. Add a Box Collider, check "Is Trigger"
///   3. Attach this script to it
///   4. Set the cost (default 3000)
/// </summary>
public class HealthUpgradeStation : MonoBehaviour
{
    [Header("Settings")]
    public int cost = 3000;
    public int newMaxHealth = 150;
    public KeyCode buyKey = KeyCode.F;

    private bool playerInRange = false;
    private PlayerHUD hud;

    void Start()
    {
        hud = FindFirstObjectByType<PlayerHUD>();
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(buyKey))
            TryBuy();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;

        if (hud != null)
        {
            int pts = PlayerStats.Instance != null ? PlayerStats.Instance.Points : 0;
            bool canAfford = pts >= cost;
            bool alreadyUpgraded = PlayerStats.Instance != null && PlayerStats.Instance.maxHealth >= newMaxHealth;
            string msg = "<b>[F] Upgrade Max Health</b>\n" +
                         "<color=" + (canAfford && !alreadyUpgraded ? "#FFD700" : "#FF4444") + ">" +
                         cost.ToString("N0") + " Points</color>";
            if (alreadyUpgraded) msg += "\n<color=#FF4444>Already upgraded!</color>";
            hud.ShowBuyPrompt(msg);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (hud != null) hud.HideBuyPrompt();
    }

    void TryBuy()
    {
        if (PlayerStats.Instance == null) return;

        if (PlayerStats.Instance.maxHealth >= newMaxHealth)
        {
            if (hud != null)
            {
                hud.ShowBuyPrompt("<b>Already purchased</b>\n<color=#FF4444>Max health upgrade owned</color>");
            }
            Debug.Log("Max health already upgraded!");
            return;
        }

        bool success = PlayerStats.Instance.SpendPoints(cost);
        if (success)
        {
            PlayerStats.Instance.maxHealth = newMaxHealth;
            // Ensure current health remains valid under the new max
            PlayerStats.Instance.SetHealth(Mathf.Min(PlayerStats.Instance.CurrentHealth, newMaxHealth));
            Debug.Log("Upgraded max health to " + newMaxHealth + "!");
        }
        else
        {
            Debug.Log("Not enough points for health upgrade. Need " + cost);
        }
    }
}