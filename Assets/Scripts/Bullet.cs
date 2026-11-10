
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 1;

    private bool hasHit;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        // Ignorar al jugador.
        if (collision.CompareTag("Player") ||
            collision.transform.root.CompareTag("Player"))
        {
            return;
        }

        // Ignorar otros triggers, como RoomTrigger.
        if (collision.isTrigger) return;

        // Buscar vida del enemigo en el objeto o sus padres.
        EnemyHealth enemy = collision.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Buscar un objeto destructible.
        DestructibleObject destructible =
            collision.GetComponentInParent<DestructibleObject>();

        if (destructible != null)
        {
            hasHit = true;
            destructible.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Paredes, rocas permanentes y otros obstáculos.
        hasHit = true;
        Destroy(gameObject);
    }
}
