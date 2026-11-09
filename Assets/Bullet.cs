using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca contra el jugador, ignoramos el choque
        if (collision.CompareTag("Player")) return;

        // Si la bala colisiona con un objeto marcado como "Enemy"
        if (collision.CompareTag("Enemy"))
        {
            // Destruimos al enemigo
            Destroy(collision.gameObject);
        }

        // Destruimos la bala al impactar con cualquier obstáculo o enemigo
        Destroy(gameObject);
    }
}