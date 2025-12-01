using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [Header("Heal Settings")]
    [SerializeField] private float healAmount = 3f;
    [SerializeField] private bool destroyAfterUse = true;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // Проверяем, нужно ли лечение
            if (playerHealth.GetCurrentHealth() < playerHealth.GetMaxHealth())
            {
                playerHealth.Heal(healAmount);
                Debug.Log($"Health pack used! Healed {healAmount} HP");

                if (destroyAfterUse)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.Log("Health is already full!");
            }
        }
    }
}