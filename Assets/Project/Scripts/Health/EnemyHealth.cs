using UnityEngine;

namespace Expedition0.Health
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 2f;
        [SerializeField] private float currentHealth;
        private bool _isDead = false;

        public bool IsDead() => _isDead;

        void Start()
        {
            currentHealth = maxHealth;
            _isDead = currentHealth > 0;
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

        public void Heal(float health)
        {
            currentHealth += Mathf.Min(maxHealth, currentHealth + health);
            
            Debug.Log($"Enemy '{gameObject.name}' healed by {health} HP. Current HP: {currentHealth}/{maxHealth}");
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
}