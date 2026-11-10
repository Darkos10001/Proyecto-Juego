
using System.Collections.Generic;
using UnityEngine;

public class RoomDecorationSpawner : MonoBehaviour
{
    [Header("Obstaculos permanentes")]
    public GameObject[] permanentPrefabs;
    public int minPermanent = 1;
    public int maxPermanent = 3;

    [Header("Objetos destructibles")]
    public GameObject[] destructiblePrefabs;
    public int minDestructible = 1;
    public int maxDestructible = 3;

    [Header("Distribucion")]
    public float wallMargin = 4f;
    public float centerClearRadius = 3.5f;
    public float placementRadius = 1.5f;
    public float minDistanceBetweenObjects = 2.5f;
    public int maxAttempts = 30;

    private readonly List<Vector2> placedPositions =
        new List<Vector2>();

    private Vector2 currentRoomCenter;
    private bool generated;

    public void GenerateDecorations(
        Vector2 roomCenter,
        bool isStartRoom)
    {
        if (generated)
            return;

        generated = true;

        currentRoomCenter = roomCenter;

        // La sala inicial permanece despejada
        if (isStartRoom)
            return;

        RoomBoundsDetector bounds =
            GetComponent<RoomBoundsDetector>();

        float width =
            bounds != null ? bounds.roomWidth : 26f;

        float height =
            bounds != null ? bounds.roomHeight : 24f;

        float halfWidth = width / 2f - wallMargin;
        float halfHeight = height / 2f - wallMargin;

        if (halfWidth <= centerClearRadius ||
            halfHeight <= centerClearRadius)
        {
            Debug.LogWarning(
                "[RoomDecorationSpawner] Sala demasiado pequena."
            );
            return;
        }

        SpawnGroup(
            permanentPrefabs,
            minPermanent,
            maxPermanent,
            halfWidth,
            halfHeight
        );

        SpawnGroup(
            destructiblePrefabs,
            minDestructible,
            maxDestructible,
            halfWidth,
            halfHeight
        );

        Debug.Log(
            $"{name}: {placedPositions.Count} obstaculos generados."
        );
    }

    void SpawnGroup(
        GameObject[] prefabs,
        int minimum,
        int maximum,
        float halfWidth,
        float halfHeight)
    {
        if (prefabs == null || prefabs.Length == 0)
            return;

        int min = Mathf.Max(0, minimum);
        int max = Mathf.Max(min, maximum);

        int count = Random.Range(min, max + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab =
                prefabs[Random.Range(0, prefabs.Length)];

            if (prefab == null)
                continue;

            bool placed = false;

            for (int attempt = 0;
                 attempt < maxAttempts;
                 attempt++)
            {
                Vector2 position = currentRoomCenter +
                    new Vector2(
                        Random.Range(-halfWidth, halfWidth),
                        Random.Range(-halfHeight, halfHeight)
                    );

                if (!IsPositionAvailable(position))
                    continue;

                float rotation = Random.Range(0f, 360f);

                GameObject obj = Instantiate(
                    prefab,
                    position,
                    Quaternion.Euler(0f, 0f, rotation),
                    transform
                );

                obj.name = prefab.name + "_Generated";

                placedPositions.Add(position);

                placed = true;
                break;
            }

            if (!placed)
            {
                Debug.LogWarning(
                    $"[RoomDecorationSpawner] No se encontro espacio " +
                    $"para colocar {prefab.name} en {name}."
                );
            }
        }
    }

    bool IsPositionAvailable(Vector2 position)
    {
        // Mantener libre el centro de la sala
        if (Vector2.Distance(
            position,
            currentRoomCenter) < centerClearRadius)
        {
            return false;
        }

        // Evitar que los obstaculos se encimen
        foreach (Vector2 existing in placedPositions)
        {
            if (Vector2.Distance(
                position,
                existing) < minDistanceBetweenObjects)
            {
                return false;
            }
        }

        // Comprobar paredes y otros colliders
        Collider2D[] overlaps =
            Physics2D.OverlapCircleAll(
                position,
                placementRadius
            );

        foreach (Collider2D col in overlaps)
        {
            if (col == null || col.isTrigger)
                continue;

            // Ignorar colliders de triggers y objetos que
            // no bloquean fisicamente el espacio.
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            Application.isPlaying
                ? (Vector3)currentRoomCenter
                : transform.position,
            centerClearRadius
        );
    }
}
