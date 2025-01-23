using System.Collections.Generic;
using UnityEngine;

public class GameSpawner : MonoBehaviour {
    [Header("Arena Settings")]
    [SerializeField] private Vector2 arenaSize = new Vector2(10, 10); // Width and Height of the arena
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float minSpawnDistance = 2f; // Minimum distance from the player

    [SerializeField] private float minSpawnInterval = 2f;
    [SerializeField] private float maxSpawnInterval = 5f; // Time between spawns

    [SerializeField] private float minAmmoSpawnInterval = 10f;
    [SerializeField] private float maxAmmoSpawnInterval = 20f;
    private float nextAmmoSpawnTime = 0f;

    [Header("Difficulty Settings")]
    [SerializeField] private float difficultyIncreaseInterval = 10f; // Time to increase difficulty
    [SerializeField] private float spawnRateMultiplier = 0.9f; // Reduce spawn interval (1 = no change)
    [SerializeField] private float enemyStatMultiplier = 1.1f; // Increase enemy stats (health, speed, etc.)
    private float nextDifficultyIncreaseTime = 0f;

    private float nextSpawnTime = 0f;

    private List<GameObject> entities = new List<GameObject>();
    private List<GameObject> ammoEntities = new List<GameObject>();

    private void Start() {
        player = GameManager.Instance.player.transform;
    }

    void Update() {
        if (Time.time >= nextSpawnTime) {
            SpawnEnemy();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        if (Time.time >= nextDifficultyIncreaseTime) {
            IncreaseDifficulty();
            nextDifficultyIncreaseTime = Time.time + difficultyIncreaseInterval;
        }

        if (Time.time >= nextAmmoSpawnTime) {
            SpawnAmmo();
            nextAmmoSpawnTime = Time.time + Random.Range(minAmmoSpawnInterval, maxAmmoSpawnInterval);
        }
    }

    void IncreaseDifficulty() {
        // Decrease spawn interval to spawn enemies faster
        minSpawnInterval *= spawnRateMultiplier;
        maxSpawnInterval *= spawnRateMultiplier;
    }

    void SpawnAmmo() {
        Vector3 spawnPosition;

        float padding = 1.0f;

        do {
            float x = Random.Range(-arenaSize.x / 2 + padding, arenaSize.x / 2 - padding);
            float y = Random.Range(-arenaSize.y / 2 + padding, arenaSize.y / 2 - padding);
            spawnPosition = new Vector3(x, y, 0);
        } while (Vector3.Distance(spawnPosition, player.position) < minSpawnDistance); // Avoid spawning too close

        var ammo = Instantiate(ammoPrefab, spawnPosition, Quaternion.identity);
        ammoEntities.Add(ammo);
    }

    void SpawnEnemy() {
        Vector3 spawnPosition;

        do {
            // Generate a random position in the arena
            float x = Random.Range(-arenaSize.x / 2, arenaSize.x / 2);
            float y = Random.Range(-arenaSize.y / 2, arenaSize.y / 2);

            spawnPosition = new Vector3(x, y, 0);
        }
        while (Vector3.Distance(spawnPosition, player.position) < minSpawnDistance); // Ensure it's not too close to the player

        // Spawn the enemy
        var ghoul = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        GhoulComponent comp = ghoul.GetComponent<GhoulComponent>();
        comp.target = player.gameObject;
        comp.movSpeed *= enemyStatMultiplier;
        entities.Add(ghoul);
    }

    public void killAll() {
        foreach (GameObject entity in entities) {
            Destroy(entity);
        }
        entities.Clear();
    }

    private void OnDrawGizmosSelected() {
        // Visualize the arena in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(arenaSize.x, arenaSize.y, 1));
    }
}
