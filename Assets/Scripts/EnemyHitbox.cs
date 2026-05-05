using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 10;
    public float attackCooldown = 1f;

    private float timer = 0f;

    void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    void OnTriggerStay(Collider other)
    {
        if (timer > 0f) return;

        if (other.CompareTag("Player"))
        {
            PlayerStats.Instance.TakeDamage(damage);
            timer = attackCooldown;
        }
    }
}
