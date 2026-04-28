using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// PlayerStats is the single source of truth for all player data:
/// health, points, ammo, and current round.
///
/// HOW TO SET UP:
///   1. Attach this script to your Player GameObject.
///   2. All other scripts reference this via PlayerStats.Instance (singleton).
///   3. Hook up the Unity Events in the Inspector if you want external reactions
///      (e.g. play a hurt sound when OnHealthChanged fires).
/// </summary>
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    // ──────────────────────────────────────────
    //  Inspector Settings
    // ──────────────────────────────────────────
    [Header("Health")]
    public int maxHealth = 100;

    [Header("Ammo")]
    public int maxAmmoInMag  = 30;
    public int maxAmmoReserve = 120;

    [Header("Starting Points")]
    public int startingPoints = 500;

    // ──────────────────────────────────────────
    //  Runtime State
    // ──────────────────────────────────────────
    public int CurrentHealth   { get; private set; }
    public int Points          { get; private set; }
    public int AmmoInMag       { get; private set; }
    public int AmmoReserve     { get; private set; }
    public int CurrentRound    { get; private set; } = 1;
    public bool IsDead         => CurrentHealth <= 0;

    // ──────────────────────────────────────────
    //  Events — subscribe in HUD or other systems
    // ──────────────────────────────────────────
    [HideInInspector] public UnityEvent OnHealthChanged  = new UnityEvent();
    [HideInInspector] public UnityEvent OnPointsChanged  = new UnityEvent();
    [HideInInspector] public UnityEvent OnAmmoChanged    = new UnityEvent();
    [HideInInspector] public UnityEvent OnRoundChanged   = new UnityEvent();
    [HideInInspector] public UnityEvent OnDeath          = new UnityEvent();

    // ──────────────────────────────────────────
    //  Lifecycle
    // ──────────────────────────────────────────
    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        CurrentHealth = maxHealth;
        Points        = startingPoints;
        AmmoInMag     = maxAmmoInMag;
        AmmoReserve   = maxAmmoReserve;
    }

    // ──────────────────────────────────────────
    //  Health
    // ──────────────────────────────────────────

    /// <summary>Call this when the player is hit by a zombie or hazard.</summary>
    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged.Invoke();

        if (CurrentHealth <= 0)
        {
            Debug.Log("Player died.");
            OnDeath.Invoke();
        }
    }

    /// <summary>Restore health (e.g. from a perk or power-up).</summary>
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged.Invoke();
    }

    // ──────────────────────────────────────────
    //  Points
    // ──────────────────────────────────────────

    /// <summary>
    /// Award points to the player (e.g. for hitting or killing a zombie).
    /// Returns true always — use AddPoints for rewards.
    /// </summary>
    public void AddPoints(int amount)
    {
        Points += amount;
        OnPointsChanged.Invoke();
    }

    /// <summary>
    /// Spend points. Returns true if the player had enough points.
    /// Returns false and does nothing if they cannot afford it.
    /// </summary>
    public bool SpendPoints(int amount)
    {
        if (Points < amount) return false;
        Points -= amount;
        OnPointsChanged.Invoke();
        return true;
    }

    // ──────────────────────────────────────────
    //  Ammo
    // ──────────────────────────────────────────

    /// <summary>Called by PlayerShoot each time a bullet is fired.</summary>
    public void UseBullet()
    {
        if (AmmoInMag <= 0) return;
        AmmoInMag--;
        OnAmmoChanged.Invoke();
    }

    /// <summary>Called when the player reloads.</summary>
    public void Reload()
    {
        int needed = maxAmmoInMag - AmmoInMag;
        int take   = Mathf.Min(needed, AmmoReserve);
        AmmoInMag    += take;
        AmmoReserve  -= take;
        OnAmmoChanged.Invoke();
    }

    /// <summary>Add reserve ammo (e.g. from picking up ammo drops).</summary>
    public void AddAmmo(int amount)
    {
        AmmoReserve = Mathf.Min(maxAmmoReserve, AmmoReserve + amount);
        OnAmmoChanged.Invoke();
    }

    // ──────────────────────────────────────────
    //  Round
    // ──────────────────────────────────────────

    /// <summary>Advance to the next round — called by EnemySpawner.</summary>
    public void AdvanceRound()
    {
        CurrentRound++;
        OnRoundChanged.Invoke();
        Debug.Log("Round " + CurrentRound + " started.");
    }
}
