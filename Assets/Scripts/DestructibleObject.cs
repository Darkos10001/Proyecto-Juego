
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [Header("Resistencia")]
    public int maxHealth = 5;

    [Header("Recompensas opcionales")]
    public GameObject dropPrefab;

    [Range(0f, 1f)]
    public float dropChance = 0.25f;

    private int currentHealth;
    private bool isDestroyed;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed || damage <= 0) return;

        currentHealth -= damage;

        Debug.Log(
            gameObject.name +
            " recibió daño. Resistencia restante: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            BreakObject();
        }
    }

  
private void BreakObject()
{
    if (isDestroyed) return;

    isDestroyed = true;

    BarrelExplosion explosion =
        GetComponent<BarrelExplosion>();

    if (explosion != null)
    {
        explosion.Explode();
    }
    else
    {
        Debug.LogWarning("Este objeto no tiene BarrelExplosion.");
    }

    if (dropPrefab != null && Random.value <= dropChance)
    {
        Instantiate(
            dropPrefab,
            transform.position,
            Quaternion.identity
        );
    }

    Destroy(gameObject);
}

}
