using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 2f;
    [SerializeField] private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Enemy '{gameObject.name}' spawned with {currentHealth} HP");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"Enemy '{gameObject.name}' took {damage} damage. Current HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    private void Die()
    {
        Debug.Log($"Enemy '{gameObject.name}' died!");
        Destroy(gameObject);
    }
}