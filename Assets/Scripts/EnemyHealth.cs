using UnityEngine;

/// <summary>
/// EnemyHealth tracks zombie hit points and notifies PlayerShoot
/// when a hit or kill occurs so the HUD and point system can react.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;

    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. HP left: " + currentHealth);

        bool killed = currentHealth <= 0;

        // Notify the shooter (for hit marker + points)
        PlayerShoot shooter = FindObjectOfType<PlayerShoot>();
        if (shooter != null)
            shooter.OnBulletHitEnemy(killedEnemy: killed);

        if (killed)
        {
            isDead = true;
            Debug.Log(gameObject.name + " died.");
            Destroy(gameObject);
        }
    }
}
