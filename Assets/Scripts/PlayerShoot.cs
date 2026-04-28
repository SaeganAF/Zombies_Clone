using UnityEngine;

/// <summary>
/// PlayerShoot handles firing bullets, ammo consumption, and reloading.
/// It now integrates with PlayerStats (ammo tracking) and PlayerHUD (hit marker).
///
/// CHANGES FROM ORIGINAL:
///   - Calls PlayerStats.Instance.UseBullet() on each shot
///   - Calls PlayerStats.Instance.Reload() on R key press
///   - Calls PlayerHUD.ShowHitMarker() when a bullet hits an EnemyHealth component
///   - Awards points to the player for hits and kills via PlayerStats
/// </summary>
public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public Transform  firePoint;
    public float      fireRate = 0.3f;

    [Header("Points")]
    [Tooltip("Points awarded for hitting a zombie")]
    public int pointsPerHit  = 10;
    [Tooltip("Points awarded for killing a zombie")]
    public int pointsPerKill = 100;

    // ──────────────────────────────────────────
    //  Private
    // ──────────────────────────────────────────
    private float     nextFireTime = 0f;
    private PlayerHUD hud;

    void Start()
    {
        hud = FindObjectOfType<PlayerHUD>();
    }

    void Update()
    {
        // ── Shoot ──────────────────────────────
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            // Check ammo before shooting
            if (PlayerStats.Instance != null && PlayerStats.Instance.AmmoInMag <= 0)
            {
                Debug.Log("Out of ammo — press R to reload.");
                return;
            }

            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // ── Reload ─────────────────────────────
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.Reload();
        }
    }

    void Shoot()
    {
        // Deduct one bullet from the magazine
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.UseBullet();

        // Spawn the bullet
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(firePoint.forward)
        );

        // Optional: use a Raycast in addition to / instead of a bullet prefab
        // for instant-hit weapons. Uncomment the section below if needed.

        /*
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 500f))
        {
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                bool wasAlive = !enemy.IsDead; // add IsDead property to EnemyHealth if needed
                enemy.TakeDamage(1);
                if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.AddPoints(pointsPerHit);
                    if (enemy == null) // destroyed = killed
                        PlayerStats.Instance.AddPoints(pointsPerKill);
                }
                if (hud != null) hud.ShowHitMarker();
            }
        }
        */
    }

    /// <summary>
    /// Call this from Bullet.cs when the bullet hits an enemy.
    /// Bullet.cs can find this via FindObjectOfType or a static reference.
    /// </summary>
    public void OnBulletHitEnemy(bool killedEnemy)
    {
        if (hud != null) hud.ShowHitMarker();

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddPoints(pointsPerHit);
            if (killedEnemy)
                PlayerStats.Instance.AddPoints(pointsPerKill);
        }
    }
}
