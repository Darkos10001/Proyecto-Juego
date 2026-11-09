using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 6; // 6 medios corazones (3 corazones enteros, estilo Isaac)
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Detectar cuando el enemigo choca físicamente contra el jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("¡Vida actual del jugador: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("¡Game Over!");
        // Aquí puedes reiniciar la escena o mostrar pantalla de muerte más adelante
        gameObject.SetActive(false);
    }
}