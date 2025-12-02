using System;
using UnityEngine;
using UnityEngine.Events;

namespace Expedition0.Health
{
    public class HealthPack : MonoBehaviour
    {
        [Header("Heal Settings")]
        [SerializeField] private float healAmount = 25f;
        [SerializeField] private bool destroyAfterUse = true;
        [SerializeField] private GameObject pickupEffect;
        [SerializeField] private UnityEvent<IDamageable> onPickup;

        private void Start()
        {
            onPickup?.AddListener(Heal);
        }

        private void OnTriggerEnter(Collider other)
        {
            // We specifically look for HealthSystem here because IDamageable 
            // usually doesn't strictly enforce a 'Heal' method, but our class does.
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                onPickup?.Invoke(damageable);
            }
        }

        public void Heal(IDamageable health)
        {
            // Only heal if not full
            if (health.GetCurrentHealth() < health.GetMaxHealth())
            {
                health.Heal(healAmount);
                
                if (pickupEffect != null)
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);

                if (destroyAfterUse)
                    Destroy(gameObject);
            }
        }

        public void HealAll()
        {
            bool healed = false;

            foreach (var player in FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None))
            {
                if (player.GetCurrentHealth() < player.GetMaxHealth())
                {
                    player.Heal(healAmount);
                    healed = true;
                }
            }

            if (healed)
            {
                if (pickupEffect != null)
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);

                if (destroyAfterUse)
                    Destroy(gameObject);
            }
        }
    }
}