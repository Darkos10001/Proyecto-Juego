using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    [Header("Prefab de la Sala")]
    public GameObject roomPrefab;     

    [Header("Configuración del Mapa")]
    public int totalRooms = 10;        

    [Header("Tamaño de las Salas")]
    public float roomWidth = 26f;     
    public float roomHeight = 24f;    

    private List<Vector2> occupiedPositions = new List<Vector2>();
    private List<GameObject> spawnedRooms = new List<GameObject>();

    void Start() {
        GenerateDungeon();
    }

    void GenerateDungeon() {
        // 1. Posición inicial (0,0)
        Vector2 currentPos = Vector2.zero;
        SpawnRoom(currentPos);
        occupiedPositions.Add(currentPos);

        // 2. Bucle para generar el resto de las habitaciones
        int safetyCounter = 0;
        for (int i = 0; i < totalRooms - 1; i++) {
            // Buscamos una sala existente en la lista para ramificar desde ella
            currentPos = occupiedPositions[Random.Range(0, occupiedPositions.Count)];
            
            Vector2 nextPos = GetNextRandomPosition(currentPos);

            int attempts = 0;
            // Intentamos encontrar una posición libre alrededor de la sala seleccionada
            while (occupiedPositions.Contains(nextPos) && attempts < 50) {
                currentPos = occupiedPositions[Random.Range(0, occupiedPositions.Count)];
                nextPos = GetNextRandomPosition(currentPos);
                attempts++;
            }

            // Si encontramos un lugar libre, colocamos la nueva sala
            if (!occupiedPositions.Contains(nextPos)) {
                SpawnRoom(nextPos);
                occupiedPositions.Add(nextPos);
            }

            safetyCounter++;
            if (safetyCounter > 500) break; // Prevenir bucles infinitos
        }
        
        Debug.Log($"[DungeonGenerator] ¡Mazmorra generada con {occupiedPositions.Count} salas!");
    }

    Vector2 GetNextRandomPosition(Vector2 fromPosition) {
        int direction = Random.Range(0, 4); // 0: Arriba, 1: Abajo, 2: Izquierda, 3: Derecha
        Vector2 newPos = fromPosition;

        if (direction == 0) newPos.y += roomHeight;
        else if (direction == 1) newPos.y -= roomHeight;
        else if (direction == 2) newPos.x -= roomWidth;
        else if (direction == 3) newPos.x += roomWidth;

        return newPos;
    }

    void SpawnRoom(Vector2 position) {
        if (roomPrefab != null) {
            GameObject newRoom = Instantiate(roomPrefab, position, Quaternion.identity);
            spawnedRooms.Add(newRoom);
            
            // Apagar los spawners de enemigos en las salas secundarias recién creadas
            Transform spawner = newRoom.transform.Find("Spawner");
            if (spawner != null && position != Vector2.zero) {
                spawner.gameObject.SetActive(false);
            }
        }
    }
}