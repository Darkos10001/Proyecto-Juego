
using System.Collections.Generic;
using UnityEngine;

public class BarrelExplosion : MonoBehaviour
{
    [Header("Explosion")]
    public float explosionRadius = 3f;
    public int explosionDamage = 2;

    [Header("Visual")]
    public GameObject explosionEffect;

    private bool hasExploded;

    public void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        Debug.Log("¡EXPLOSION DEL BARRIL!");

        if (explosionEffect != null)
        {
            Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius
        );

        Debug.Log("Objetos detectados: " + hits.Length);

        HashSet<EnemyHealth> enemies = new HashSet<EnemyHealth>();
        HashSet<PlayerHealth> players = new HashSet<PlayerHealth>();
        HashSet<DestructibleObject> objects =
            new HashSet<DestructibleObject>();

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy =
                hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null && enemies.Add(enemy))
            {
                enemy.TakeDamage(explosionDamage);
                Debug.Log("Enemigo alcanzado: " + enemy.name);
            }

            PlayerHealth player =
                hit.GetComponentInParent<PlayerHealth>();

            if (player != null && players.Add(player))
            {
                player.TakeDamage(explosionDamage);
                Debug.Log("Jugador alcanzado por explosión");
            }

            DestructibleObject destructible =
                hit.GetComponentInParent<DestructibleObject>();

            if (destructible != null && objects.Add(destructible))
            {
                if (destructible.gameObject != gameObject)
                {
                    destructible.TakeDamage(explosionDamage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
