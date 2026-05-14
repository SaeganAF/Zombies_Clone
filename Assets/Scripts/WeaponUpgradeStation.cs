using UnityEngine;


public class WeaponUpgradeStation : MonoBehaviour
{
    [Header("Settings")]
    public int cost = 4000;
    public int newMaxAmmoInMag = 45;
    public int newMaxAmmoReserve = 240;
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
            bool alreadyUpgraded = PlayerStats.Instance != null && PlayerStats.Instance.isWeaponUpgraded;
            string msg = "<b>[F] Upgrade Weapon</b>\n" +
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

        if (PlayerStats.Instance.isWeaponUpgraded)
        {
            if (hud != null)
            {
                hud.ShowBuyPrompt("<b>Already purchased</b>\n<color=#FF4444>Weapon upgrade owned</color>");
            }
            Debug.Log("Weapon already upgraded!");
            return;
        }

        bool success = PlayerStats.Instance.SpendPoints(cost);
        if (success)
        {
            PlayerStats.Instance.isWeaponUpgraded = true;
            PlayerStats.Instance.UpgradeAmmoCaps(newMaxAmmoInMag, newMaxAmmoReserve);
            PlayerStats.Instance.OnWeaponUpgraded.Invoke();
            Debug.Log("Upgraded weapon! New ammo capacities: " + newMaxAmmoInMag + " / " + newMaxAmmoReserve);
        }
        else
        {
            Debug.Log("Not enough points for weapon upgrade. Need " + cost);
        }
    }
}