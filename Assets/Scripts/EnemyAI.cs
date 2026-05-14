using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 3.5f;

    /// <summary>
    /// How fast the enemy moves (units per second).
    /// The player walks at 5 and runs at 8, so 3 means the enemy
    /// is always slower than the player — you can always escape by walking.
    /// Raise this value to make the game harder.
    /// </summary>
    public float moveSpeed = 5f;

    [Header("Attack Settings")]

    /// <summary>
    /// How close the enemy must be before it can attack the player (in Unity units).
    /// </summary>
    public float attackRange = 1.5f;

    /// <summary>
    /// Seconds between each attack. Prevents the enemy from dealing damage every frame.
    /// </summary>
    public float attackCooldown = 1f;

    /// <summary>
    /// How much health the player loses per hit. Hook this into your health system later.
    /// </summary>
    public int damagePerHit = 10;

    // Internal references — found automatically, you don't set these in the Inspector
    private NavMeshAgent agent;
    private Transform playerTransform;

    // Tracks time so we only attack once per attackCooldown seconds
    private float attackTimer = 1f;

    void Start()
    {
        // Get the NavMeshAgent attached to this same GameObject
        agent = GetComponent<NavMeshAgent>();

        // Apply our speed setting to the agent
        agent.speed = baseSpeed * DifficultyManager.Instance.GetSpeedMultiplier();


        // Prevent the NavMeshAgent from rotating the enemy itself;
        // we handle facing toward the player manually in Update
        agent.angularSpeed = 360f;

        // Find the player in the scene using their tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("EnemyAI on " + gameObject.name + ": No GameObject tagged 'Player' found. " +
                             "Select your Player in the Hierarchy and set its Tag to 'Player'.");
        }
    }

    /*void Update()
    {
        if (playerTransform == null) return;

        // Always chase the player
        agent.isStopped = false;
        agent.SetDestination(playerTransform.position);

        // Always face the player
        FacePlayer();
    }
    */


    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                AttackPlayer();
                attackTimer = attackCooldown;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            attackTimer = attackCooldown;
        }

        FacePlayer();
    }
    



    /// <summary>
    /// Smoothly rotates the enemy to look at the player.
    /// </summary>
    void FacePlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0f; // Keep rotation flat (no tilting up/down)

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Slerp smoothly rotates toward the player — 10f controls rotation speed
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }


    void AttackPlayer()
{
    // Deal damage to the player using your PlayerStats singleton
    if (PlayerStats.Instance != null)
    {
        PlayerStats.Instance.TakeDamage(damagePerHit);
    }

    Debug.Log("Enemy attacked the player for " + damagePerHit + " damage!");
}


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
