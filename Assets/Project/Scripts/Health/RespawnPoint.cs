using UnityEngine;

namespace Expedition0.Health
{
    public class RespawnPoint : MonoBehaviour
    {
        [Header("Respawn Point Settings")] [SerializeField]
        private string pointName = "Checkpoint";
    
        [SerializeField] private bool isActive;
    
        [Header("Visuals")] [SerializeField] private GameObject activeVisual;
    
        [SerializeField] private GameObject inactiveVisual;
    
        [Header("Audio")] [SerializeField] private AudioClip activationSound;
    
        private void Start()
        {
            UpdateVisuals();
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = isActive ? Color.green : Color.gray;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
            Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "RespawnPoint.png", true);
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null && !isActive) ActivateRespawnPoint(playerHealth);
            }
        }
    
        private void ActivateRespawnPoint(PlayerHealth playerPlayerHealth)
        {
            isActive = true;
            playerPlayerHealth.SetRespawnPoint(transform.position, transform.rotation);
    
            DeactivateOtherRespawnPoints();
    
    
            UpdateVisuals();
    
    
            if (activationSound != null) AudioSource.PlayClipAtPoint(activationSound, transform.position);
    
            Debug.Log($": {pointName}");
        }
    
        private void DeactivateOtherRespawnPoints()
        {
            var allPoints = FindObjectsByType<RespawnPoint>(FindObjectsSortMode.None);
            foreach (var point in allPoints)
                if (point != this)
                    point.Deactivate();
        }
    
        public void Deactivate()
        {
            isActive = false;
            UpdateVisuals();
        }
    
        private void UpdateVisuals()
        {
            if (activeVisual != null)
                activeVisual.SetActive(isActive);
    
            if (inactiveVisual != null)
                inactiveVisual.SetActive(!isActive);
        }
    }
}