using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 2.5f; // Velocidad de movimiento del enemigo
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Buscar automáticamente al jugador mediante su Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // Moverse suavemente hacia la posición del jugador usando físicas
            Vector2 targetPosition = Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(targetPosition);
        }
    }
}