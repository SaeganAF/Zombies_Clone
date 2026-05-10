using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

/// <summary>
/// EnemyAI makes an enemy automatically chase the player using Unity's NavMesh system.
///
/// The NavMesh is a "map" that Unity bakes (pre-calculates) from your scene's geometry.
/// It tells the NavMeshAgent exactly which areas are walkable, so the enemy will
/// automatically walk AROUND walls, pillars, and other obstacles instead of getting stuck.
///
/// ─────────────────────────────────────────────────────────────
///  HOW TO CREATE THE ENEMY PREFAB (step by step)
/// ─────────────────────────────────────────────────────────────
///  1. In the Hierarchy, right-click → 3D Object → Capsule.
///     Rename it "Enemy".
///
///  2. Give it a colour so it's easy to spot:
///     a. In the Project window → right-click → Create → Material. Name it "EnemyMaterial".
///     b. In the Inspector for that material, click the Albedo colour box and pick red/green/etc.
///     c. Drag the material from the Project window onto the Enemy capsule in the Scene view.
///
///  3. Add the NavMeshAgent component:
///     Select the Enemy in the Hierarchy → In the Inspector click "Add Component"
///     → search for "Nav Mesh Agent" → click it to add.
///     This is what allows the enemy to navigate around obstacles.
///
///  4. Add THIS script (EnemyAI):
///     With the Enemy still selected → "Add Component" → search "EnemyAI" → click it.
///
///  5. Set the Enemy's Tag:
///     At the top of the Inspector, click the "Tag" dropdown → "Add Tag..."
///     → click the + button → type "Enemy" → Save.
///     Then select your Enemy again and set its Tag to "Enemy".
///
///  6. Make it a Prefab:
///     Drag the Enemy from the Hierarchy down into the Assets/Prefabs folder in the
///     Project window. A blue cube icon will appear — that's your Prefab!
///     You can now delete the original from the Hierarchy (the Prefab is saved).
///
/// ─────────────────────────────────────────────────────────────
///  HOW TO BAKE THE NAVMESH (so enemies can navigate)
/// ─────────────────────────────────────────────────────────────
///  The NavMesh must be baked from your scene's floor/walls BEFORE the enemies can navigate.
///
///  1. Select every wall, floor, and ceiling piece in the Hierarchy.
///     (Click the first one, then Shift+Click the last, or Ctrl+Click individual ones.)
///
///  2. In the Inspector, click "Add Component" → search "NavMesh Surface".
///     If you don't see it, it is in the AI Navigation package:
///       → Window → Package Manager → search "AI Navigation" → Install it.
///
///     Alternatively, use the older bake method:
///       → Window → AI → Navigation (Obsolete)
///       → Click the "Bake" tab → click the "Bake" button.
///       A blue overlay will appear on walkable surfaces — that's the NavMesh.
///
///  3. Mark static geometry:
///     Select all floor/wall objects, and at the top-right of the Inspector
///     check the "Static" checkbox (or the dropdown → "Navigation Static").
///     This tells Unity to include them in the NavMesh bake.
///
/// ─────────────────────────────────────────────────────────────
///  HOW TO CONNECT EVERYTHING IN THE SPAWNER
/// ─────────────────────────────────────────────────────────────
///  1. Select your EnemySpawner GameObject in the Hierarchy.
///  2. Drag the Enemy Prefab from Assets/Prefabs into the "Enemy Prefab" slot.
///  3. Make sure your Player GameObject has its Tag set to "Player".
///  4. Press Play — enemies will spawn and chase you!
///
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]

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
        agent.speed = moveSpeed;

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

    /// <summary>
    /// Called when the enemy is close enough to hit the player.
    /// Currently just logs a message — replace this with your health system later.
    ///
    /// EXAMPLE of how to deal damage once you have a PlayerHealth script:
    ///   PlayerHealth health = playerTransform.GetComponent&lt;PlayerHealth&gt;();
    ///   if (health != null) health.TakeDamage(damagePerHit);
    /// </summary>
    void AttackPlayer()
{
    // Deal damage to the player using your PlayerStats singleton
    if (PlayerStats.Instance != null)
    {
        PlayerStats.Instance.TakeDamage(damagePerHit);
    }

    Debug.Log("Enemy attacked the player for " + damagePerHit + " damage!");
}


    /// <summary>
    /// Draws a red sphere in the Scene view (not visible in Game view) showing attack range.
    /// This is a helpful visual debug tool — no gameplay impact.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
