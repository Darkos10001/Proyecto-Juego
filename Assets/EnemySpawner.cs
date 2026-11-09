using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [Header("Oleada aleatoria")]
    public int minEnemies = 3;               // Mínimo de enemigos de la oleada
    public int maxEnemies = 7;               // Máximo de enemigos de la oleada (incluido)
    public float spawnInterval = 0.5f;       // Segundos entre enemigo y enemigo
    public float minDistanceFromPlayer = 3f; // Para que no aparezcan encima del jugador

    [Header("Área de aparición (RELATIVA a la posición de este objeto)")]
    public float minX = -7f, maxX = 7f;
    public float minY = -3.5f, maxY = 3.5f;

    // Se dispara una sola vez, cuando ya aparecieron todos y todos fueron eliminados
    public event System.Action AllEnemiesDefeated;

    private readonly List<GameObject> spawned = new List<GameObject>();
    private int enemiesToSpawn;
    private int spawnedCount;
    private float timer;
    private bool finished;
    private Transform player;

    void Start()
    {
        // Tamaño de la oleada al azar (el límite superior de Random.Range es exclusivo)
        int low = Mathf.Min(minEnemies, maxEnemies);
        int high = Mathf.Max(minEnemies, maxEnemies);
        enemiesToSpawn = Random.Range(low, high + 1);

        // El primer enemigo aparece de inmediato
        timer = spawnInterval;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (finished || enemyPrefab == null) return;

        // Fase 1: ir generando la oleada
        if (spawnedCount < enemiesToSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnEnemy();
            }
            return;
        }

        // Fase 2: ya aparecieron todos; esperar a que mueran
        spawned.RemoveAll(e => e == null); // un enemigo destruido cuenta como null en Unity
        if (spawned.Count == 0)
        {
            finished = true;
            AllEnemiesDefeated?.Invoke();
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
        spawned.Add(enemy);
        spawnedCount++;
    }

    Vector2 GetSpawnPosition()
    {
        Vector2 pos = transform.position;

        // Hasta 10 intentos para encontrar un punto lejos del jugador
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            pos = (Vector2)transform.position + offset;

            if (player == null || Vector2.Distance(pos, player.position) >= minDistanceFromPlayer)
                return pos;
        }
        return pos;
    }
}
