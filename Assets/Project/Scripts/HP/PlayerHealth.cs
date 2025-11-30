using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Player spawned with {currentHealth} HP");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"Player took {damage} damage. Current HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        float healedAmount = Mathf.Min(amount, maxHealth - currentHealth);
        currentHealth += healedAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"Player healed {healedAmount} HP. Current HP: {currentHealth}/{maxHealth}");
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
        Debug.Log("PLAYER DIED!");
        // Здесь можно добавить логику смерти игрока
        // Например: перезагрузка уровня, экран смерти и т.д.
    }
}