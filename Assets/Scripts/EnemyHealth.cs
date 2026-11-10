
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida del enemigo")]
    public int maxHealth = 3;

    private int currentHealth;
    private bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        currentHealth -= damage;

        Debug.Log(
            gameObject.name + " recibió daño. Vida restante: " + currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Destroy(gameObject);
    }
}
