using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f; // Cada cuántos segundos aparece un enemigo
    private float spawnTimer;

    public float minX = -7f, maxX = 7f;
    public float minY = -3.5f, maxY = 3.5f;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // Generar una posición aleatoria dentro de los límites de la sala
        Vector2 randomPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        }
    }
}