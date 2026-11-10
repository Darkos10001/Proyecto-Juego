
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject enemyPrefab;

    [Header("Configuracion del mapa")]
    public int totalRooms = 10;

    [Header("Tamano de las salas")]
    public float roomWidth = 26f;
    public float roomHeight = 24f;

    [Header("Puertas")]
    public float doorWidth = 6f;
    public Color doorColor = new Color(0.95f, 0.75f, 0.2f);

    [Header("Oleadas aleatorias")]
    public int minEnemies = 3;
    public int maxEnemies = 7;
    public float spawnInterval = 0.5f;
    public float spawnMargin = 3f;

    private readonly List<Vector2Int> cells =
        new List<Vector2Int>();

    private readonly HashSet<Vector2Int> occupied =
        new HashSet<Vector2Int>();

    private readonly List<GameObject> spawnedRooms =
        new List<GameObject>();

    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    void Start()
    {
        if (roomPrefab == null)
        {
            Debug.LogError("[DungeonGenerator] Falta roomPrefab.");
            return;
        }

        DisableLooseSpawners();

        if (enemyPrefab == null)
        {
            Debug.LogError("[DungeonGenerator] Falta enemyPrefab.");
        }

        GenerateLayout();

        foreach (Vector2Int cell in cells)
        {
            SpawnRoom(cell);
        }

        Debug.Log(
            $"[DungeonGenerator] Mazmorra generada: {cells.Count} salas."
        );
    }

    void DisableLooseSpawners()
    {
        EnemySpawner[] looseSpawners =
            FindObjectsByType<EnemySpawner>();

        foreach (EnemySpawner s in looseSpawners)
        {
            if (s.GetComponentInParent<Room>() != null)
                continue;

            if (enemyPrefab == null && s.enemyPrefab != null)
            {
                enemyPrefab = s.enemyPrefab;

                Debug.LogWarning(
                    "[DungeonGenerator] Se reutilizo el enemyPrefab del Spawner."
                );
            }

            s.gameObject.SetActive(false);
        }
    }

    void GenerateLayout()
    {
        cells.Clear();
        occupied.Clear();

        cells.Add(Vector2Int.zero);
        occupied.Add(Vector2Int.zero);

        int safetyCounter = 0;

        while (cells.Count < totalRooms && safetyCounter < 1000)
        {
            safetyCounter++;

            Vector2Int from =
                cells[Random.Range(0, cells.Count)];

            Vector2Int next =
                from + directions[
                    Random.Range(0, directions.Length)
                ];

            if (occupied.Add(next))
            {
                cells.Add(next);
            }
        }
    }

    Vector2 GetPrefabCenterOffset()
    {
        Transform l = roomPrefab.transform.Find("Wall_Left");
        Transform r = roomPrefab.transform.Find("Wall_Right");
        Transform t = roomPrefab.transform.Find("Wall_Top");
        Transform b = roomPrefab.transform.Find("Wall_Bottom");

        if (l == null || r == null || t == null || b == null)
            return Vector2.zero;

        return new Vector2(
            (l.localPosition.x + r.localPosition.x) / 2f,
            (t.localPosition.y + b.localPosition.y) / 2f
        );
    }

    void SpawnRoom(Vector2Int cell)
    {
        Vector2 center = new Vector2(
            cell.x * roomWidth,
            cell.y * roomHeight
        );

        GameObject newRoom = Instantiate(
            roomPrefab,
            center - GetPrefabCenterOffset(),
            Quaternion.identity
        );

        newRoom.name = $"Room_{cell.x}_{cell.y}";

        spawnedRooms.Add(newRoom);

        Room room = newRoom.GetComponent<Room>();

        if (room == null)
        {
            Debug.LogError(
                $"[DungeonGenerator] {newRoom.name} no tiene componente Room."
            );
            return;
        }

        bool isStartRoom = cell == Vector2Int.zero;

        room.isCleared = isStartRoom;

        room.BuildDoorways(
            occupied.Contains(cell + Vector2Int.up),
            occupied.Contains(cell + Vector2Int.down),
            occupied.Contains(cell + Vector2Int.left),
            occupied.Contains(cell + Vector2Int.right),
            doorWidth,
            doorColor
        );

        if (!isStartRoom)
        {
            SetupSpawner(newRoom, center);
        }

        // GENERAR OBSTACULOS ALEATORIOS
        RoomDecorationSpawner decorations =
            newRoom.GetComponent<RoomDecorationSpawner>();

        if (decorations != null)
        {
            decorations.GenerateDecorations(
                center,
                isStartRoom
            );
        }

        RoomBoundsDetector bounds =
            newRoom.GetComponent<RoomBoundsDetector>();

        if (isStartRoom && bounds != null &&
            (Mathf.Abs(bounds.roomWidth - roomWidth) > 0.1f ||
             Mathf.Abs(bounds.roomHeight - roomHeight) > 0.1f))
        {
            Debug.LogWarning(
                $"[DungeonGenerator] El prefab mide " +
                $"{bounds.roomWidth}x{bounds.roomHeight}, " +
                $"pero se configuraron {roomWidth}x{roomHeight}."
            );
        }
    }

    void SetupSpawner(GameObject newRoom, Vector2 worldCenter)
    {
        Transform spawnerTransform =
            newRoom.transform.Find("Spawner");

        GameObject spawnerObj;

        if (spawnerTransform != null)
        {
            spawnerObj = spawnerTransform.gameObject;
        }
        else
        {
            spawnerObj = new GameObject("Spawner");
            spawnerObj.transform.SetParent(newRoom.transform);
        }

        spawnerObj.transform.position = worldCenter;

        EnemySpawner spawner =
            spawnerObj.GetComponent<EnemySpawner>();

        if (spawner == null)
        {
            spawner = spawnerObj.AddComponent<EnemySpawner>();
        }

        if (spawner.enemyPrefab == null)
        {
            spawner.enemyPrefab = enemyPrefab;
        }

        spawner.minEnemies = minEnemies;
        spawner.maxEnemies = maxEnemies;
        spawner.spawnInterval = spawnInterval;

        RoomBoundsDetector bounds =
            newRoom.GetComponent<RoomBoundsDetector>();

        float w = bounds != null ? bounds.roomWidth : roomWidth;
        float h = bounds != null ? bounds.roomHeight : roomHeight;

        float halfW = Mathf.Max(1f, w / 2f - spawnMargin);
        float halfH = Mathf.Max(1f, h / 2f - spawnMargin);

        spawner.minX = -halfW;
        spawner.maxX = halfW;
        spawner.minY = -halfH;
        spawner.maxY = halfH;

        // Se activa cuando el jugador entra en la sala
        spawnerObj.SetActive(false);
    }
}
