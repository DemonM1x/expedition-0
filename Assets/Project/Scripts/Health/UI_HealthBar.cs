using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar Components")] [SerializeField]
    private Slider healthSlider;

    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Color Settings")] [SerializeField]
    private Color fullHealthColor = Color.green;

    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f;

    [Header("Animation Settings")] [SerializeField]
    private float animationSpeed = 2f;

    private float currentDisplayHealth;
    private float targetHealth;

    private void Start()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        currentDisplayHealth = healthSlider.maxValue;
        targetHealth = healthSlider.maxValue;
    }

    private void Update()
    {
        // Плавно анимируем к целевому значению
        currentDisplayHealth = Mathf.Lerp(currentDisplayHealth, targetHealth, Time.deltaTime * animationSpeed);

        if (healthSlider != null)
        {
            healthSlider.value = currentDisplayHealth;
            UpdateHealthColor();
        }

        if (healthText != null) healthText.text = $"{Mathf.RoundToInt(currentDisplayHealth)}";
    }

    public void SetMaxHealth(float maxHealth)
    {
        Debug.Log($"HealthBar: SetMaxHealth({maxHealth})");

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            targetHealth = maxHealth;
            currentDisplayHealth = maxHealth;
            healthSlider.value = maxHealth; // Мгновенно устанавливаем максимальное здоровье
            Debug.Log($"HealthBar: Successfully set max health to {maxHealth}");
        }
        else
        {
            Debug.LogError("HealthBar: healthSlider is null! Cannot set max health.");
        }

        UpdateHealthText();
    }

    public void SetHealth(float health)
    {
        Debug.Log($"HealthBar: SetHealth({health}) - Current target: {targetHealth}, Display: {currentDisplayHealth}");

        // Устанавливаем целевое значение для плавной анимации
        targetHealth = health;

        if (healthSlider == null)
        {
            Debug.LogError("HealthBar: healthSlider is null! Cannot set health.");
            return;
        }

        Debug.Log($"HealthBar: Target health updated to {targetHealth}");
        UpdateHealthText();
    }

    private void UpdateHealthColor()
    {
        if (fillImage == null) return;

        var healthPercentage = healthSlider.value / healthSlider.maxValue;

        if (healthPercentage <= lowHealthThreshold)
            fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercentage / lowHealthThreshold);
        else
            fillImage.color = fullHealthColor;
    }

    private void UpdateHealthText()
    {
        if (healthText != null) healthText.text = $"{Mathf.RoundToInt(targetHealth)}";
    }

    /// <summary>
    ///     Мгновенно устанавливает здоровье без анимации
    /// </summary>
    public void SetHealthInstant(float health)
    {
        targetHealth = health;
        currentDisplayHealth = health;

        if (healthSlider != null) healthSlider.value = health;

        UpdateHealthText();
    }

    public float GetTargetHealth()
    {
        return targetHealth;
    }

    public float GetDisplayHealth()
    {
        return currentDisplayHealth;
    }


    public bool IsAnimationComplete()
    {
        return Mathf.Approximately(currentDisplayHealth, targetHealth);
    }

    [ContextMenu("Test Set Health 50")]
    public void TestSetHealth50()
    {
        SetHealth(50f);
    }

    [ContextMenu("Test Set Health 25")]
    public void TestSetHealth25()
    {
        SetHealth(25f);
    }

    [ContextMenu("Test Set Health 100")]
    public void TestSetHealthFull()
    {
        SetHealth(healthSlider.maxValue);
    }

    [ContextMenu("Test Set Health Instant 0")]
    public void TestSetHealthInstant0()
    {
        SetHealthInstant(0f);
    }

    [ContextMenu("Print Health Info")]
    public void PrintHealthInfo()
    {
        Debug.Log("=== HEALTH BAR INFO ===");
        Debug.Log($"Target Health: {targetHealth}");
        Debug.Log($"Display Health: {currentDisplayHealth}");
        Debug.Log($"Max Health: {healthSlider.maxValue}");
        Debug.Log($"Animation Complete: {IsAnimationComplete()}");
        Debug.Log($"Health Percentage: {targetHealth / healthSlider.maxValue:P}");
    }
}