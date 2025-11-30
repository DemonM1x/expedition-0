using UnityEngine;

public class HealthSystemTester : MonoBehaviour
{
    [Header("Test References")]
    [SerializeField] private HealthSystem targetHealth;
    
    [Header("Test Settings")]
    [SerializeField] private float testDamageAmount = 25f;
    [SerializeField] private float testHealAmount = 15f;
    
    private void Start()
    {
        if (targetHealth == null)
            targetHealth = FindObjectOfType<HealthSystem>();
    }
    
    private void Update()
    {
        // Горячие клавиши для тестирования
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TestDamage();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            TestHeal();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            TestKill();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestRespawn();
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            PrintHealthInfo();
        }
    }
    
    [ContextMenu("Test Damage")]
    public void TestDamage()
    {
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(testDamageAmount);
            Debug.Log($"Applied {testDamageAmount} damage. Current health: {targetHealth.GetCurrentHealth()}");
        }
        else
        {
            Debug.LogWarning("No HealthSystem found to test!");
        }
    }
    
    [ContextMenu("Test Heal")]
    public void TestHeal()
    {
        if (targetHealth != null)
        {
            targetHealth.Heal(testHealAmount);
            Debug.Log($"Healed {testHealAmount}. Current health: {targetHealth.GetCurrentHealth()}");
        }
        else
        {
            Debug.LogWarning("No HealthSystem found to test!");
        }
    }
    
    [ContextMenu("Test Kill")]
    public void TestKill()
    {
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(targetHealth.GetCurrentHealth());
            Debug.Log("Applied lethal damage!");
        }
        else
        {
            Debug.LogWarning("No HealthSystem found to test!");
        }
    }
    
    [ContextMenu("Test Respawn")]
    public void TestRespawn()
    {
        if (targetHealth != null)
        {
            Vector3 respawnPos = transform.position + Vector3.up * 2f;
            targetHealth.RespawnAtPosition(respawnPos, Quaternion.identity);
            Debug.Log($"Respawned at position: {respawnPos}");
        }
        else
        {
            Debug.LogWarning("No HealthSystem found to test!");
        }
    }
    
    [ContextMenu("Print Health Info")]
    public void PrintHealthInfo()
    {
        if (targetHealth != null)
        {
            Debug.Log($"=== HEALTH INFO ===");
            Debug.Log($"Current Health: {targetHealth.GetCurrentHealth()}");
            Debug.Log($"Max Health: {targetHealth.GetMaxHealth()}");
            Debug.Log($"Health Percentage: {targetHealth.GetHealthPercentage():P}");
            Debug.Log($"Is Dead: {targetHealth.IsDead()}");
        }
        else
        {
            Debug.LogWarning("No HealthSystem found to test!");
        }
    }
    
    private void OnGUI()
    {
        if (targetHealth == null) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== HEALTH SYSTEM TESTER ===");
        GUILayout.Label($"Health: {targetHealth.GetCurrentHealth():F1} / {targetHealth.GetMaxHealth()}");
        GUILayout.Label($"Percentage: {targetHealth.GetHealthPercentage():P}");
        GUILayout.Label($"Is Dead: {targetHealth.IsDead()}");
        
        GUILayout.Space(10);
        GUILayout.Label("Controls:");
        GUILayout.Label("Q - Damage | E - Heal | R - Kill | T - Respawn | I - Info");
        
        GUILayout.Space(10);
        if (GUILayout.Button($"Damage ({testDamageAmount})"))
        {
            TestDamage();
        }
        
        if (GUILayout.Button($"Heal ({testHealAmount})"))
        {
            TestHeal();
        }
        
        if (GUILayout.Button("Kill"))
        {
            TestKill();
        }
        
        GUILayout.EndArea();
    }
}