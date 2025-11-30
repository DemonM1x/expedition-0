using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("UI Reference")]
    [SerializeField] private HealthBar healthBar;
    
    [Header("Events")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDeath;
    public UnityEvent OnRespawn;
    
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        // Автоматически найти HealthBar если не назначен
        if (healthBar == null)
        {
            healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            {
                Debug.Log("HealthSystem: Automatically found HealthBar component");
            }
            else
            {
                Debug.LogWarning("HealthSystem: No HealthBar found in scene!");
            }
        }

        respawnPosition = transform.position;
        respawnRotation = transform.rotation;

        if (healthBar != null)
        {
            Debug.Log($"HealthSystem: Initializing HealthBar with maxHealth={maxHealth}, currentHealth={currentHealth}");
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            Debug.LogError("HealthSystem: Cannot initialize - healthBar is null!");
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        float previousHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"HealthSystem: TakeDamage({damage}) - Health: {previousHealth} -> {currentHealth}");

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
            Debug.Log($"HealthSystem: Called healthBar.SetHealth({currentHealth})");
        }
        else
        {
            Debug.LogWarning("HealthSystem: healthBar is null! Cannot update UI.");
        }

        OnTakeDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        float previousHealth = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"HealthSystem: Heal({healAmount}) - Health: {previousHealth} -> {currentHealth}");
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
            Debug.Log($"HealthSystem: Called healthBar.SetHealth({currentHealth})");
        }
        else
        {
            Debug.LogWarning("HealthSystem: healthBar is null! Cannot update UI.");
        }
    }
    
    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} ����!");
        

        OnDeath?.Invoke();
        

        Invoke("Respawn", 3f);
    }
    
    private void Respawn()
    {
        string respawnScene = GetRespawnScene();
        
        if (SceneManager.GetActiveScene().name != respawnScene)
        {
            SceneManager.LoadScene(respawnScene);
        }
        else
        {
            RespawnAtPosition(respawnPosition, respawnRotation);
        }
        
        currentHealth = maxHealth;
        isDead = false;
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        OnRespawn?.Invoke();
        
        Debug.Log($"{gameObject.name}!");
    }
    
    private string GetRespawnScene()
    {
        bool tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        
        if (!tutorialCompleted)
        {
            return "Bridge Corridor";
        }
        else
        {
            return "Lobby_Default";
        }
    }
    
    public void RespawnAtPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        
        currentHealth = maxHealth;
        isDead = false;
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        
        OnRespawn?.Invoke();
    }
    
    public void SetRespawnPoint(Vector3 position, Quaternion rotation)
    {
        respawnPosition = position;
        respawnRotation = rotation;
    }
    
    public bool IsDead() => isDead;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
}