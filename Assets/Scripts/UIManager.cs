using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public TextMeshProUGUI healthText;

    void Update()
    {
        if (playerHealth != null && healthText != null)
        {
            healthText.text = "Vida: " + playerHealth.currentHealth + " / " + playerHealth.maxHealth;
        }
    }
}