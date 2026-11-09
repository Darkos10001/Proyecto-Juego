using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [Header("Oleada")]
    public int enemiesToSpawn = 5;          // Cuántos enemigos aparecen en total en esta sala
    public float spawnInterval = 2f;        // Segundos entre enemigo y enemigo
    public float minDistanceFromPlayer = 3f; // Para que no aparezcan encima del jugador

    [Header("Área de aparición (RELATIVA a la posición de este objeto)")]
    public float minX = -7f, maxX = 7f;
    public float minY = -3.5f, maxY = 3.5f;

    // Se dispara una sola vez, cuando ya aparecieron todos y todos fueron eliminados
    public event System.Action AllEnemiesDefeated;

    private readonly List<GameObject> spawned = new List<GameObject>();
    private int spawnedCount;
    private float timer;
    private bool finished;
    private Transform player;

    void Start()
    {
        // El primer enemigo aparece de inmediato
        timer = spawnInterval;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (finished || enemyPrefab == null) return;

        // Fase 1: ir generando enemigos
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
