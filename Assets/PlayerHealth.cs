using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 6; // 6 medios corazones (3 corazones enteros, estilo Isaac)
    public int currentHealth;

    [Header("Daño")]
    public float invulnerabilityTime = 1f; // Segundos sin recibir daño tras un golpe
    public float restartDelay = 2f;        // Segundos antes de reiniciar al morir

    private float invulnerableUntil;
    private bool isDead;
    private SpriteRenderer sprite;

    void Start()
    {
        currentHealth = maxHealth;
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Golpe inicial al tocar al enemigo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContact(collision);
    }

    // Si el enemigo sigue pegado, vuelve a dañar cuando termina la invulnerabilidad
    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleContact(collision);
    }

    void HandleContact(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || Time.time < invulnerableUntil) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        invulnerableUntil = Time.time + invulnerabilityTime;
        Debug.Log("¡Vida actual del jugador: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Flash());
        }
    }

    // Parpadeo mientras dura la invulnerabilidad
    IEnumerator Flash()
    {
        if (sprite == null) yield break;

        while (Time.time < invulnerableUntil)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        sprite.enabled = true;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("¡Game Over!");

        // No desactivamos el GameObject: así el Invoke de abajo sí se ejecuta.
        if (sprite != null) sprite.enabled = false;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        PlayerShooting shooting = GetComponent<PlayerShooting>();
        if (shooting != null) shooting.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Invoke(nameof(RestartScene), restartDelay);
    }

    void RestartScene()
    {
        // La escena debe estar en File > Build Profiles (Build Settings)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
