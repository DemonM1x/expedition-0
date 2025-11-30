using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private bool destroyOnDamage = false;
    [SerializeField] private LayerMask targetLayers = -1;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject hitEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, transform.rotation);
                }
                
                if (destroyOnDamage)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, collision.contacts[0].point, Quaternion.identity);
                }
                
                if (destroyOnDamage)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}