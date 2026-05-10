using UnityEngine;

/// <summary>
/// AmmoBox lets the player buy ammo for 1000 points by pressing F near it.
///
/// HOW TO SET UP:
///   1. Create a cube in the Hierarchy (3D Object -> Cube), rename it "AmmoBox"
///   2. Add a Box Collider, check "Is Trigger"
///   3. Attach this script to it
///   4. Set the cost (default 1000)
/// </summary>
public class AmmoBox : MonoBehaviour
{
    [Header("Settings")]
    public int cost = 1000;
    public int ammoAmount = 60;
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
            string msg = "<b>[F] Buy Ammo</b>\n" +
                         "<color=" + (canAfford ? "#FFD700" : "#FF4444") + ">" +
                         cost.ToString("N0") + " Points</color>";
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

        bool success = PlayerStats.Instance.SpendPoints(cost);
        if (success)
        {
            PlayerStats.Instance.AddAmmo(ammoAmount);
            Debug.Log("Bought ammo! +" + ammoAmount);
        }
        else
        {
            Debug.Log("Not enough points for ammo. Need " + cost);
        }
    }
}