
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs de enemigos")]
    public GameObject enemyPrefab;
    public GameObject powerfulEnemyPrefab;

    [Header("Probabilidad de enemigos poderosos")]
    [Range(0f, 100f)]
    public float powerfulEnemyChance = 20f;

    [Header("Cantidad de enemigos")]
    public int minEnemies = 3;
    public int maxEnemies = 7;

    [Header("Tiempo de aparicion")]
    public float spawnInterval = 0.5f;

    [Header("Distancia del jugador")]
    public float minDistanceFromPlayer = 3f;

    [Header("Limites de la sala")]
    public float minX = -7f;
    public float maxX = 7f;
    public float minY = -3.5f;
    public float maxY = 3.5f;

    [Header("Evitar obstaculos")]
    public float spawnCheckRadius = 0.8f;
    public int maxSpawnAttempts = 30;

    public event System.Action AllEnemiesDefeated;

    private readonly List<GameObject> spawnedEnemies =
        new List<GameObject>();

    private int enemiesToSpawn;
    private int spawnedCount;

    private float spawnTimer;
    private bool finished;

    private Transform player;

    void Start()
    {
        enemiesToSpawn = Random.Range(
            Mathf.Max(1, minEnemies),
            Mathf.Max(minEnemies, maxEnemies) + 1
        );

        spawnTimer = 0f;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError(
                "[EnemySpawner] No hay Enemy Prefab asignado."
            );

            FinishWave();
            return;
        }

        Debug.Log(
            "[EnemySpawner] Oleada iniciada. Enemigos: " +
            enemiesToSpawn
        );
    }

    void Update()
    {
        if (finished)
            return;

        spawnedEnemies.RemoveAll(enemy => enemy == null);

        // Generar solamente la cantidad establecida
        if (spawnedCount < enemiesToSpawn)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                bool success = SpawnEnemy();

                if (success)
                {
                    spawnedCount++;
                    spawnTimer = spawnInterval;
                }
                else
                {
                    // Reintentar si no se encontro espacio
                    spawnTimer = 0.5f;
                }
            }
        }

        // Terminar cuando todos hayan aparecido y sido derrotados
        if (spawnedCount >= enemiesToSpawn &&
            spawnedEnemies.Count == 0)
        {
            FinishWave();
        }
    }

    bool SpawnEnemy()
    {
        if (enemyPrefab == null)
            return false;

        Vector2 spawnPosition;

        if (!TryGetSpawnPosition(out spawnPosition))
        {
            Debug.LogWarning(
                "[EnemySpawner] No se encontro espacio libre."
            );

            return false;
        }

        GameObject selectedPrefab = enemyPrefab;

        if (powerfulEnemyPrefab != null &&
            Random.Range(0f, 100f) < powerfulEnemyChance)
        {
            selectedPrefab = powerfulEnemyPrefab;
        }

        GameObject enemy = Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );

        spawnedEnemies.Add(enemy);

        Debug.Log(
            "[EnemySpawner] Aparecio " +
            selectedPrefab.name +
            " (" + spawnedCount + 1 +
            "/" + enemiesToSpawn + ")"
        );

        return true;
    }

    bool TryGetSpawnPosition(out Vector2 validPosition)
    {
        validPosition = Vector2.zero;

        for (int attempt = 0;
             attempt < maxSpawnAttempts;
             attempt++)
        {
            Vector2 candidate =
                (Vector2)transform.position +
                new Vector2(
                    Random.Range(minX, maxX),
                    Random.Range(minY, maxY)
                );

            if (player != null)
            {
                float distance = Vector2.Distance(
                    candidate,
                    player.position
                );

                if (distance < minDistanceFromPlayer)
                    continue;
            }

            if (!IsSpawnPositionFree(candidate))
                continue;

            validPosition = candidate;
            return true;
        }

        return false;
    }

    bool IsSpawnPositionFree(Vector2 position)
    {
        Collider2D[] overlaps =
            Physics2D.OverlapCircleAll(
                position,
                spawnCheckRadius
            );

        foreach (Collider2D col in overlaps)
        {
            if (col == null || col.isTrigger)
                continue;

            return false;
        }

        return true;
    }

    void FinishWave()
    {
        if (finished)
            return;

        finished = true;

        Debug.Log(
            "[EnemySpawner] Oleada completada. Sala despejada."
        );

        AllEnemiesDefeated?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = transform.position +
            new Vector3(
                (minX + maxX) / 2f,
                (minY + maxY) / 2f,
                0f
            );

        Vector3 size = new Vector3(
            maxX - minX,
            maxY - minY,
            0f
        );

        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            spawnCheckRadius
        );
    }
}
