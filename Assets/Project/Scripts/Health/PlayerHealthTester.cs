using UnityEngine;
using UnityEngine.Serialization;

namespace Expedition0.Health
{
    public class PlayerHealthTester : MonoBehaviour
{
    [Header("Test Settings")] [SerializeField]
    private float testDamageAmount = 25f;

    [SerializeField] private float testHealAmount = 15f;

    [FormerlySerializedAs("targetHealth")] [Header("Test References")] [SerializeField]
    private PlayerHealth targetPlayerHealth;

    private void Start()
    {
        if (targetPlayerHealth == null)
            targetPlayerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    private void Update()
    {
        // Горячие клавиши для тестирования
        if (Input.GetKeyDown(KeyCode.Q)) TestDamage();

        if (Input.GetKeyDown(KeyCode.E)) TestHeal();

        if (Input.GetKeyDown(KeyCode.R)) TestKill();

        if (Input.GetKeyDown(KeyCode.T)) TestRespawn();

        if (Input.GetKeyDown(KeyCode.I)) PrintHealthInfo();
    }

    private void OnGUI()
    {
        if (targetPlayerHealth == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== HEALTH SYSTEM TESTER ===");
        GUILayout.Label($"Health: {targetPlayerHealth.GetCurrentHealth():F1} / {targetPlayerHealth.GetMaxHealth()}");
        GUILayout.Label($"Percentage: {targetPlayerHealth.GetHealthPercentage():P}");
        GUILayout.Label($"Is Dead: {targetPlayerHealth.IsDead()}");

        GUILayout.Space(10);
        GUILayout.Label("Controls:");
        GUILayout.Label("Q - Damage | E - Heal | R - Kill | T - Respawn | I - Info");

        GUILayout.Space(10);
        if (GUILayout.Button($"Damage ({testDamageAmount})")) TestDamage();

        if (GUILayout.Button($"Heal ({testHealAmount})")) TestHeal();

        if (GUILayout.Button("Kill")) TestKill();

        GUILayout.EndArea();
    }

    [ContextMenu("Test Damage")]
    public void TestDamage()
    {
        if (targetPlayerHealth != null)
        {
            targetPlayerHealth.TakeDamage(testDamageAmount);
            Debug.Log($"Applied {testDamageAmount} damage. Current health: {targetPlayerHealth.GetCurrentHealth()}");
        }
        else
        {
            Debug.LogWarning("No HealthSystem found to test!");
        }
    }

    [ContextMenu("Test Heal")]
    public void TestHeal()
    {
        if (targetPlayerHealth != null)
        {
            targetPlayerHealth.Heal(testHealAmount);
            Debug.Log($"Healed {testHealAmount}. Current health: {targetPlayerHealth.GetCurrentHealth()}");
        }
        else
        {
            Debug.LogWarning("No HealthSystem found to test!");
        }
    }

    [ContextMenu("Test Kill")]
    public void TestKill()
    {
        if (targetPlayerHealth != null)
        {
            targetPlayerHealth.TakeDamage(targetPlayerHealth.GetCurrentHealth());
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
        if (targetPlayerHealth != null)
        {
            var respawnPos = transform.position + Vector3.up * 2f;
            targetPlayerHealth.RespawnAtPosition(respawnPos, Quaternion.identity);
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
        if (targetPlayerHealth != null)
        {
            Debug.Log("=== HEALTH INFO ===");
            Debug.Log($"Current Health: {targetPlayerHealth.GetCurrentHealth()}");
            Debug.Log($"Max Health: {targetPlayerHealth.GetMaxHealth()}");
            Debug.Log($"Health Percentage: {targetPlayerHealth.GetHealthPercentage():P}");
            Debug.Log($"Is Dead: {targetPlayerHealth.IsDead()}");
        }
        else
        {
            Debug.LogWarning("No HealthSystem found to test!");
        }
    }
}
}
