using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        // Guardamos la referencia al Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Capturamos el movimiento con las teclas WASD o flechas de dirección
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalizamos el vector para que no camine más rápido en diagonal
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Aplicamos el movimiento mediante físicas
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}