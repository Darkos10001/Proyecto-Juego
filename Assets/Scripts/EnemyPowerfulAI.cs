
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPowerfulAI : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 2.8f;
    public float arriveDistance = 0.5f;

    [Header("Busqueda de cobertura")]
    public float coverSearchRadius = 9f;
    public float coverDistance = 1.1f;
    public float bodyCheckRadius = 0.45f;
    public float repositionInterval = 3f;

    [Header("Comportamiento")]
    public float coverHoldTime = 1.5f;
    public float advanceTime = 1.5f;

    private Rigidbody2D rb;
    private Transform player;

    private Vector2 targetPosition;

    private bool hasCover;
    private bool advancing;

    private float behaviorTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        behaviorTimer = Random.Range(0f, 1f);
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        behaviorTimer -= Time.fixedDeltaTime;

        if (behaviorTimer <= 0f)
        {
            UpdateBehavior();
        }

        MoveTowardsTarget();
    }

    void UpdateBehavior()
    {
        if (advancing)
        {
            advancing = false;
            FindCover();
            behaviorTimer = repositionInterval;
            return;
        }

        if (hasCover)
        {
            advancing = true;
            targetPosition = player.position;
            behaviorTimer = advanceTime;
            return;
        }

        FindCover();

        if (!hasCover)
        {
            targetPosition = player.position;
            behaviorTimer = repositionInterval;
        }
    }

    void FindCover()
    {
        Collider2D[] obstacles =
            Physics2D.OverlapCircleAll(
                player.position,
                coverSearchRadius
            );

        float bestScore = float.MaxValue;
        bool found = false;

        foreach (Collider2D obstacle in obstacles)
        {
            if (obstacle == null || obstacle.isTrigger)
                continue;

            if (obstacle.transform.root == transform.root)
                continue;

            if (obstacle.CompareTag("Player") ||
                obstacle.CompareTag("Enemy"))
                continue;

            Vector2 obstaclePosition = obstacle.bounds.center;

            Vector2 awayFromPlayer =
                (obstaclePosition - (Vector2)player.position).normalized;

            if (awayFromPlayer == Vector2.zero)
                continue;

            // Calcula la distancia necesaria para salir del obstaculo
            Bounds bounds = obstacle.bounds;

            float obstacleExtent =
                Mathf.Abs(awayFromPlayer.x) * bounds.extents.x +
                Mathf.Abs(awayFromPlayer.y) * bounds.extents.y;

            Vector2 candidate =
                obstaclePosition +
                awayFromPlayer * (obstacleExtent + coverDistance);

            if (!IsPositionFree(candidate, obstacle))
                continue;

            float score = Vector2.Distance(
                rb.position,
                candidate
            );

            if (score < bestScore)
            {
                bestScore = score;
                targetPosition = candidate;
                found = true;
            }
        }

        hasCover = found;

        if (found)
        {
            Debug.Log(
                "[EnemyPowerfulAI] Cobertura encontrada: " + name
            );
        }
    }

    bool IsPositionFree(
        Vector2 position,
        Collider2D coverObstacle)
    {
        Collider2D[] overlaps =
            Physics2D.OverlapCircleAll(
                position,
                bodyCheckRadius
            );

        foreach (Collider2D col in overlaps)
        {
            if (col == null || col.isTrigger)
                continue;

            if (col == coverObstacle)
                continue;

            if (col.transform.root == transform.root)
                continue;

            if (col.CompareTag("Player") ||
                col.CompareTag("Enemy"))
                continue;

            return false;
        }

        return true;
    }

    void MoveTowardsTarget()
    {
        Vector2 direction =
            targetPosition - rb.position;

        if (direction.magnitude <= arriveDistance)
            return;

        Vector2 nextPosition =
            rb.position +
            direction.normalized *
            moveSpeed *
            Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            coverSearchRadius
        );

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            targetPosition,
            0.4f
        );
    }
}
