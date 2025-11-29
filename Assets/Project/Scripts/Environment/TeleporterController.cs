using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace Expedition0.Environment
{
    public sealed class TeleporterController : MonoBehaviour
    {
        [Header("Anchors")]
        [SerializeField] private TeleportationAnchor poleAnchor;      // on the pole
        [SerializeField] private TeleportationAnchor platformAnchor;  // on the platform (its teleportTransform is a child)

        [Header("Emission via MPB")]
        [SerializeField] private Transform emissiveRoot;              // parent of all LOD children (defaults to this.transform)

        [SerializeField] private Color baseColor = Color.lavender; 
        [SerializeField] private string emissionColorProperty = "_EmissionColor"; // your shader property (e.g. "_Emission", "_EmissionStrength")
        [SerializeField] private float activeEmission = 2f;
        [SerializeField] private float inactiveEmission = 0f;

        [Header("State")]
        [SerializeField] private bool startActive = true;

        [Header("Events")]
        [SerializeField] private UnityEvent onActivated;
        [SerializeField] private UnityEvent onDeactivated;

        public bool IsActive { get; private set; }

        private Renderer[] _renderers;
        private MaterialPropertyBlock _mpb;

        private void Awake()
        {
            if (!emissiveRoot) emissiveRoot = transform;
            _renderers = emissiveRoot.GetComponentsInChildren<Renderer>(true); // include inactive LODs
            _mpb = new MaterialPropertyBlock();

            SetActive(startActive, false);
        }

        [ContextMenu("Activate")]   public void Activate()   => SetActive(true);
        [ContextMenu("Deactivate")] public void Deactivate() => SetActive(false);
        public void Toggle() => SetActive(!IsActive);

        public void SetActive(bool active) => SetActive(active, true);

        private void SetActive(bool active, bool invokeEvents)
        {
            IsActive = active;

            // Toggle anchors (and their colliders if present) so rays can/can’t select them
            ToggleAnchor(poleAnchor, active);
            ToggleAnchor(platformAnchor, active);

            // Push emission power to all renderers via MPB (affects all LOD children)
            ApplyEmission(active ? activeEmission : inactiveEmission);

            if (!invokeEvents) return;
            if (active) onActivated?.Invoke();
            else onDeactivated?.Invoke();
        }

        private static void ToggleAnchor(TeleportationAnchor anchor, bool enable)
        {
            if (!anchor) return;
            anchor.enabled = enable;
        }

        private void ApplyEmission(float value)
        {
            if (_renderers == null || _renderers.Length == 0)
                _renderers = emissiveRoot.GetComponentsInChildren<Renderer>(true);
            
            var finalColor = baseColor * value;

            for (int i = 0; i < _renderers.Length; i++)
            {
                var r = _renderers[i];
                if (!r) continue;
                r.GetPropertyBlock(_mpb);
                _mpb.SetColor(emissionColorProperty, finalColor);
                r.SetPropertyBlock(_mpb);
            }
        }
    }
}
