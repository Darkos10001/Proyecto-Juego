
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("PlayerMovement necesita un Rigidbody2D.");
        }
    }

    void Update()
    {
        // Movimiento exclusivamente con WASD.
        movement.x = 0f;
        movement.y = 0f;

        if (Input.GetKey(KeyCode.A))
            movement.x = -1f;

        if (Input.GetKey(KeyCode.D))
            movement.x = 1f;

        if (Input.GetKey(KeyCode.W))
            movement.y = 1f;

        if (Input.GetKey(KeyCode.S))
            movement.y = -1f;

        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        rb.MovePosition(
            rb.position + movement * moveSpeed * Time.fixedDeltaTime
        );
    }
}
