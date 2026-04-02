using UnityEngine;

/// <summary>
/// EnemySpawner spawns enemies at designated spawn points at a set interval,
/// up to a maximum cap. Attach this script to an empty GameObject in your scene
/// (e.g. name it "EnemySpawner").
///
/// HOW TO SET UP SPAWN POINTS:
///   1. In the Hierarchy, right-click → Create Empty. Name it "SpawnPoint_1".
///   2. Move it to a corner or doorway of your map (somewhere the player cannot see directly).
///   3. Repeat to create as many spawn points as you want.
///   4. Select the EnemySpawner GameObject, then drag each SpawnPoint into the
///      "Spawn Points" list in the Inspector.
///
/// HOW TO SET UP THE ENEMY PREFAB:
///   - See EnemyAI.cs for full prefab setup instructions.
///   - Once you have the prefab ready, drag it into the "Enemy Prefab" slot here.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]

    /// <summary>
    /// The enemy prefab to clone each time a new enemy is spawned.
    /// Drag your Enemy prefab from the Project window into this slot.
    /// </summary>
    public GameObject enemyPrefab;

    /// <summary>
    /// Seconds between each spawn attempt. Lower = more frequent spawns.
    /// </summary>
    public float spawnInterval = 2f;

    /// <summary>
    /// How many enemies can exist in the scene at the same time.
    /// Once this limit is reached no new enemies spawn until one is destroyed.
    /// </summary>
    public int maxEnemies = 10;

    [Header("Spawn Points")]

    /// <summary>
    /// An array of Transform positions that enemies can spawn at.
    /// These are empty GameObjects you place around the edge of your map.
    /// Add them in the Inspector by expanding this list and dragging GameObjects in.
    /// </summary>
    public Transform[] spawnPoints;

    /// <summary>
    /// Minimum distance from the player that a spawn point must be to be used.
    /// This stops enemies from spawning directly on top of the player.
    /// </summary>
    public float minSpawnDistanceFromPlayer = 8f;

    // Cached reference to the player — found automatically at Start
    private Transform playerTransform;

    void Start()
    {
        // Find the player in the scene by their tag.
        // Make sure your Player GameObject has the "Player" tag set in the Inspector.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            playerTransform = playerObject.transform;
        else
            Debug.LogWarning("EnemySpawner: No GameObject with tag 'Player' found. " +
                             "Select your Player in the Hierarchy and set its Tag to 'Player'.");

        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogWarning("EnemySpawner: No spawn points assigned. " +
                             "Drag empty GameObjects into the Spawn Points list in the Inspector.");

        // Start spawning: wait 1 second, then call SpawnEnemy every spawnInterval seconds
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        // Don't spawn if the prefab or spawn points are missing
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        // Count how many enemies are currently alive in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length >= maxEnemies)
            return;

        // Build a list of spawn points that are far enough from the player
        System.Collections.Generic.List<Transform> validPoints =
            new System.Collections.Generic.List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            // If we have a player reference, skip spawn points that are too close
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(point.position, playerTransform.position);
                if (distance < minSpawnDistanceFromPlayer) continue;
            }

            validPoints.Add(point);
        }

        // If every spawn point is too close to the player, use any available point
        if (validPoints.Count == 0)
        {
            foreach (Transform point in spawnPoints)
                if (point != null) validPoints.Add(point);
        }

        if (validPoints.Count == 0) return;

        // Pick a random valid spawn point
        Transform chosenPoint = validPoints[Random.Range(0, validPoints.Count)];

        // Spawn the enemy at the chosen point with no rotation
        Instantiate(enemyPrefab, chosenPoint.position, Quaternion.identity);
    }
}