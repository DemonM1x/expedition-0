using UnityEngine;

namespace Expedition0.Health
{
    public class DamageDealer : MonoBehaviour
    {
        [Header("Damage Settings")] [SerializeField]
        private float damageAmount = 10f;

        [SerializeField] private bool destroyOnDamage;
        [SerializeField] private LayerMask targetLayers = -1;

        [Header("Visual Effects")] [SerializeField]
        private GameObject hitEffect;

        private void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & targetLayers) != 0)
            {
                var health = collision.gameObject.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damageAmount);

                    if (hitEffect != null) Instantiate(hitEffect, collision.contacts[0].point, Quaternion.identity);

                    if (destroyOnDamage) Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & targetLayers) != 0)
            {
                var health = other.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damageAmount);

                    if (hitEffect != null) Instantiate(hitEffect, transform.position, transform.rotation);

                    if (destroyOnDamage) Destroy(gameObject);
                }
            }
        }
    }
}