using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BlasterShoot : MonoBehaviour
{
    [Header("Shoot Settings")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Material beamMaterial;
    [SerializeField] private float beamLength = 10f;
    [SerializeField] private float beamWidth = 0.1f;
    [SerializeField] private float beamDuration = 0.1f;

    [Header("Damage Settings")]
    [SerializeField] private float damage = 1f;

    [Header("Layer Detection")]
    [SerializeField] private LayerMask hitLayers;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.activated.AddListener(OnActivated);
    }

    void OnDisable()
    {
        grabInteractable.activated.RemoveListener(OnActivated);
    }

    private void OnActivated(ActivateEventArgs args)
    {
        Shoot();
    }

    void Shoot()
    {
        if (shootPoint == null || beamMaterial == null)
        {
            Debug.LogWarning("ShootPoint or BeamMaterial is not assigned!");
            return;
        }

        // Создаём луч
        GameObject beam = new GameObject("Beam");
        beam.transform.position = shootPoint.position;
        beam.transform.rotation = shootPoint.rotation;

        // Добавляем LineRenderer
        LineRenderer lineRenderer = beam.AddComponent<LineRenderer>();
        lineRenderer.material = beamMaterial;
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;
        lineRenderer.positionCount = 2;

        // Raycast для проверки попаданий
        RaycastHit hit;
        Vector3 endPoint;

        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, beamLength, hitLayers))
        {
            endPoint = hit.point;

            // Пытаемся нанести урон через интерфейс
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            // Проверяем слой для логов
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log($"Hit Player: {hit.collider.gameObject.name}");
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Debug.Log($"Hit Enemy: {hit.collider.gameObject.name}");
            }
        }
        else
        {
            endPoint = shootPoint.position + shootPoint.forward * beamLength;
        }

        // Устанавливаем точки луча
        lineRenderer.SetPosition(0, shootPoint.position);
        lineRenderer.SetPosition(1, endPoint);

        // Уничтожаем луч через заданное время
        Destroy(beam, beamDuration);
    }
}