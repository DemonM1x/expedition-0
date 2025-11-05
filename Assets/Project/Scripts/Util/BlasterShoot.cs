using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit; // Если используешь XRI

/// <summary>
/// BlasterShoot — скрипт для стрельбы лазером из бластера.
/// Повесь на GameObject бластера.
/// Работает в VR с XR Device Simulator.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class BlasterShoot : MonoBehaviour
{
    [Header("🔫 Лазер")]
    [SerializeField] private Transform firePoint;      // Child-объект "Nozzle" или ствол (точка вылета). Если пусто — использует transform.
    [SerializeField] private float maxLength = 20f;     // Длина луча (м)
    [SerializeField] private LayerMask hitLayers = -1;  // Слои для Raycast (игнорирует UI/игрока)

    [Header("✨ Материал (создай сам)")]
    [SerializeField] private Material laserMaterial;    // Красный glowing материал (см. инструкцию ниже)

    [Header("⌨️ Ввод (для теста)")]
    [SerializeField] private InputActionReference shootAction; // Назначь в Inspector (Keyboard/t или XR Trigger)

    private LineRenderer laser;
    private bool isShooting;

    void Awake()
    {
        // Добавляем/настраиваем LineRenderer
        laser = GetComponent<LineRenderer>();
        SetupLaser();

        // Включаем Input Action
        if (shootAction != null)
            shootAction.action.Enable();
    }

    void OnDestroy()
    {
        if (shootAction != null)
            shootAction.action.Disable();
    }

    void Update()
    {
        // Проверяем нажатие (hold для continuous shoot)
        if (shootAction != null && shootAction.action.IsPressed())
        {
            Shoot();
        }
        else if (isShooting)
        {
            StopShoot();
        }
    }

    void Shoot()
    {
        if (!isShooting)
        {
            isShooting = true;
            laser.enabled = true;
        }
        UpdateLaser();
    }

    void StopShoot()
    {
        isShooting = false;
        laser.enabled = false;
    }

    void UpdateLaser()
    {
        Transform fp = firePoint ?? transform;
        Vector3 start = fp.position;
        Vector3 direction = fp.forward;
        Vector3 end;

        // Raycast: луч останавливается на стенах/врагах (20м макс)
        if (Physics.Raycast(start, direction, out RaycastHit hit, maxLength, hitLayers))
        {
            end = hit.point;
            // TODO: Здесь добавь урон/взрыв: hit.collider.GetComponent<Enemy>().TakeDamage();
        }
        else
        {
            end = start + direction * maxLength;
        }

        laser.SetPosition(0, start);
        laser.SetPosition(1, end);
    }

    void SetupLaser()
    {
        laser.useWorldSpace = true;
        laser.positionCount = 2;
        laser.startWidth = 0.015f;  // Толщина начала
        laser.endWidth = 0.008f;    // Конец (заостряется)
        laser.material = laserMaterial ?? CreateDefaultMaterial();
        laser.colorGradient = new Gradient
        {
            colorKeys = new[] {
                new GradientColorKey(Color.red, 0f),
                new GradientColorKey(Color.red, 1f)
            },
            alphaKeys = new[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        };
        laser.enabled = false;
    }

    Material CreateDefaultMaterial()
    {
        // Авто-создаёт красный glowing материал (URP)
        Shader unlit = Shader.Find("Universal Render Pipeline/Unlit");
        Material mat = new Material(unlit);
        mat.SetColor("_BaseColor", Color.red);
        mat.SetColor("_EmissionColor", Color.red * 5f); // Яркость glow
        mat.EnableKeyword("_EMISSION");
        return mat;
    }
}