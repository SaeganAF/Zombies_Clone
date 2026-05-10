using UnityEngine;

/// <summary>
/// HealthStation lets the player buy full health restoration for points by pressing F near it.
///
/// HOW TO SET UP:
///   1. Create a cube in the Hierarchy (3D Object -> Cube), rename it "HealthStation"
///   2. Add a Box Collider, check "Is Trigger"
///   3. Attach this script to it
///   4. Set the cost (default 1500)
/// </summary>
public class HealthStation : MonoBehaviour
{
    [Header("Settings")]
    public int cost = 1500;
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
            string msg = "<b>[F] Restore Health</b>\n" +
                         "<color=" + (canAfford ? "#FFD700" : "#FF4444") + ">" +
                         cost.ToString("N0") + " Points</color>";

            if (PlayerStats.Instance != null && PlayerStats.Instance.CurrentHealth >= PlayerStats.Instance.maxHealth)
                msg = "<b>Health Full</b>\n<color=#FF4444>Max health reached</color>";

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

        if (PlayerStats.Instance.CurrentHealth >= PlayerStats.Instance.maxHealth)
        {
            if (hud != null)
                hud.ShowBuyPrompt("<b>Health Full</b>\n<color=#FF4444>Max health reached</color>");
            Debug.Log("Cannot restore health because player is already at full health.");
            return;
        }

        bool success = PlayerStats.Instance.SpendPoints(cost);
        if (success)
        {
            PlayerStats.Instance.SetHealth(PlayerStats.Instance.maxHealth);
            Debug.Log("Restored health to full!");
        }
        else
        {
            Debug.Log("Not enough points for health restoration. Need " + cost);
        }
    }
}