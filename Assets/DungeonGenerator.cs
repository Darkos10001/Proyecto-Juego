using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject enemyPrefab;    // Arrastra aquí Enemy.prefab: se usa para crear el Spawner de cada sala

    [Header("Configuración del Mapa")]
    public int totalRooms = 10;

    [Header("Tamaño de las Salas")]
    public float roomWidth = 26f;
    public float roomHeight = 24f;

    [Header("Puertas")]
    public float doorWidth = 6f;      // El jugador mide 3 de ancho, así que debe ser mayor que eso
    public Color doorColor = new Color(0.95f, 0.75f, 0.2f);

    [Header("Oleada aleatoria por sala")]
    public int minEnemies = 3;
    public int maxEnemies = 7;
    public float spawnInterval = 0.5f;
    public float spawnMargin = 3f;    // Distancia mínima de los enemigos a las paredes

    // El mapa se calcula en una cuadrícula de enteros (evita errores de decimales)
    private readonly List<Vector2Int> cells = new List<Vector2Int>();
    private readonly HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
    private readonly List<GameObject> spawnedRooms = new List<GameObject>();

    private static readonly Vector2Int[] directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    void Start() {
        if (roomPrefab == null) {
            Debug.LogError("[DungeonGenerator] Falta asignar roomPrefab.");
            return;
        }
        if (enemyPrefab == null) {
            Debug.LogWarning("[DungeonGenerator] Falta asignar enemyPrefab: las salas no tendrán enemigos.");
        }

        GenerateLayout();

        // Primero se decide dónde va cada sala; después se crean, para saber qué vecinos tiene cada una
        foreach (Vector2Int cell in cells) {
            SpawnRoom(cell);
        }

        Debug.Log($"[DungeonGenerator] ¡Mazmorra generada con {cells.Count} salas!");
    }

    void GenerateLayout() {
        // Sala inicial en la celda (0,0)
        cells.Add(Vector2Int.zero);
        occupied.Add(Vector2Int.zero);

        int safetyCounter = 0;
        while (cells.Count < totalRooms && safetyCounter < 1000) {
            safetyCounter++;

            // Elegimos una sala existente y probamos a ramificar en una dirección al azar
            Vector2Int from = cells[Random.Range(0, cells.Count)];
            Vector2Int next = from + directions[Random.Range(0, directions.Length)];

            if (occupied.Add(next)) {
                cells.Add(next);
            }
        }
    }

    // El prefab Room_Base tiene su centro desplazado respecto a su pivote.
    // Medimos ese desplazamiento con las paredes para colocar el CENTRO de la sala donde toca.
    Vector2 GetPrefabCenterOffset() {
        Transform l = roomPrefab.transform.Find("Wall_Left");
        Transform r = roomPrefab.transform.Find("Wall_Right");
        Transform t = roomPrefab.transform.Find("Wall_Top");
        Transform b = roomPrefab.transform.Find("Wall_Bottom");
        if (l == null || r == null || t == null || b == null) return Vector2.zero;

        return new Vector2(
            (l.localPosition.x + r.localPosition.x) / 2f,
            (t.localPosition.y + b.localPosition.y) / 2f);
    }

    void SpawnRoom(Vector2Int cell) {
        Vector2 center = new Vector2(cell.x * roomWidth, cell.y * roomHeight);
        GameObject newRoom = Instantiate(roomPrefab, center - GetPrefabCenterOffset(), Quaternion.identity);
        newRoom.name = $"Room_{cell.x}_{cell.y}";
        spawnedRooms.Add(newRoom);

        Room room = newRoom.GetComponent<Room>();
        if (room == null) return;

        bool isStartRoom = (cell == Vector2Int.zero);

        // La sala inicial nace limpia para no encerrar al jugador al empezar
        room.isCleared = isStartRoom;

        // Pasillos: solo hay puerta donde existe una sala vecina
        room.BuildDoorways(
            occupied.Contains(cell + Vector2Int.up),
            occupied.Contains(cell + Vector2Int.down),
            occupied.Contains(cell + Vector2Int.left),
            occupied.Contains(cell + Vector2Int.right),
            doorWidth, doorColor);

        if (!isStartRoom) {
            SetupSpawner(newRoom, center);
        }

        // Aviso si el tamaño configurado no coincide con el tamaño real de las paredes
        RoomBoundsDetector bounds = newRoom.GetComponent<RoomBoundsDetector>();
        if (isStartRoom && bounds != null &&
            (Mathf.Abs(bounds.roomWidth - roomWidth) > 0.1f || Mathf.Abs(bounds.roomHeight - roomHeight) > 0.1f)) {
            Debug.LogWarning($"[DungeonGenerator] El prefab mide {bounds.roomWidth}x{bounds.roomHeight} " +
                             $"pero roomWidth/roomHeight valen {roomWidth}x{roomHeight}. Las salas no encajarán bien.");
        }
    }

    void SetupSpawner(GameObject newRoom, Vector2 worldCenter) {
        // Si el prefab ya trae un hijo "Spawner" lo usamos; si no, lo creamos
        Transform spawnerTransform = newRoom.transform.Find("Spawner");
        GameObject spawnerObj;
        if (spawnerTransform != null) {
            spawnerObj = spawnerTransform.gameObject;
        } else {
            spawnerObj = new GameObject("Spawner");
            spawnerObj.transform.SetParent(newRoom.transform);
        }
        spawnerObj.transform.position = worldCenter;

        EnemySpawner spawner = spawnerObj.GetComponent<EnemySpawner>();
        if (spawner == null) spawner = spawnerObj.AddComponent<EnemySpawner>();

        if (spawner.enemyPrefab == null) spawner.enemyPrefab = enemyPrefab;
        spawner.minEnemies = minEnemies;
        spawner.maxEnemies = maxEnemies;
        spawner.spawnInterval = spawnInterval;

        // Zona de aparición: toda la sala menos un margen junto a las paredes
        RoomBoundsDetector bounds = newRoom.GetComponent<RoomBoundsDetector>();
        float w = bounds != null ? bounds.roomWidth : roomWidth;
        float h = bounds != null ? bounds.roomHeight : roomHeight;
        float halfW = Mathf.Max(1f, w / 2f - spawnMargin);
        float halfH = Mathf.Max(1f, h / 2f - spawnMargin);
        spawner.minX = -halfW; spawner.maxX = halfW;
        spawner.minY = -halfH; spawner.maxY = halfH;

        // Apagado hasta que el jugador entre (RoomTrigger lo activa)
        spawnerObj.SetActive(false);
    }
}
