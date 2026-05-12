using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Player Settings")]
    public float minSpawnDistanceFromPlayer = 8f;

    private float spawnTimer;
    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogError("EnemySpawner: No Player found with tag 'Player'.");
    }

    void Update()
    {
        if (playerTransform == null) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
    }

    void TrySpawnEnemy()
    {
        // Check enemy count
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemies)
            return;

        List<Transform> validPoints = new List<Transform>();

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn == null) continue;

            // Skip disabled spawn points
            if (!spawn.gameObject.activeInHierarchy)
                continue;

            // Skip spawn points behind closed doors
            var link = spawn.GetComponent<SpawnPointDoorLink>();
            if (link != null && link.linkedDoor != null && !link.linkedDoor.isOpen)
                continue;

            // Skip spawn points too close to the player
            float distance = Vector3.Distance(spawn.position, playerTransform.position);
            if (distance < minSpawnDistanceFromPlayer)
                continue;

            validPoints.Add(spawn);
        }

        if (validPoints.Count == 0)
            return;

        // Pick a random valid spawn point
        Transform chosen = validPoints[Random.Range(0, validPoints.Count)];

        Instantiate(enemyPrefab, chosen.position, chosen.rotation);
    }
}
