using UnityEngine;


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


    private float     nextFireTime = 0f;
    private PlayerHUD hud;

    void Start()
    {
        hud = FindObjectOfType<PlayerHUD>();
        if (PlayerStats.Instance != null)
        {
            if (PlayerStats.Instance.isWeaponUpgraded)
            {
                fireRate = 0.15f;
            }
            PlayerStats.Instance.OnWeaponUpgraded.AddListener(() => fireRate = 0.15f);
        }
    }

    void Update()
    {
        // ── Shoot 
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

       
    }


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
