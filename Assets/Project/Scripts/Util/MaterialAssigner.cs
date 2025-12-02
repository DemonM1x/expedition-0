using UnityEngine;

namespace Expedition0.Util
{
    public sealed class MaterialAssigner : MonoBehaviour
    {
        [Header("Target shared material (do NOT assign per-instance copies)")]
        [SerializeField] private Material _material;   // serialized field, not a property

        [Header("Scope")]
        [SerializeField] private bool _includeInactive = true;
        [SerializeField] private bool _applyToSkinned = true;

        [Header("Slots")]
        [Tooltip("-1 = all slots, otherwise zero-based material slot index")]
        [SerializeField] private int _slot = -1;

        [Header("Safety")]
        [Tooltip("If OFF, this will duplicate materials at runtime (usually not what you want).")]
        [SerializeField] private bool _useSharedMaterials = true;

        public Material CurrentMaterial
        {
            get => _material;
            set { _material = value; Apply(); }
        }

        // private void OnEnable()
        // {
        //     if (!Application.isPlaying) Apply();
        // }

        private void Start() => Apply();

        [ContextMenu("Apply")]
        public void Apply() => AssignMaterialToChildren(_material);

        public void AssignMaterialToChildren(Material mat)
        {
            if (mat == null) return;

            var renderers = GetComponentsInChildren<Renderer>(_includeInactive);
            foreach (var r in renderers)
            {
                if (!_applyToSkinned && r is SkinnedMeshRenderer) continue;

                if (_slot < 0)
                {
                    if (_useSharedMaterials)
                    {
                        var arr = r.sharedMaterials;
                        for (int i = 0; i < arr.Length; i++) arr[i] = mat;
                        r.sharedMaterials = arr;
                    }
                    else
                    {
                        var arr = r.materials; // WARNING: makes material instances
                        for (int i = 0; i < arr.Length; i++) arr[i] = mat;
                        r.materials = arr;
                    }
                }
                else
                {
                    if (_useSharedMaterials)
                    {
                        var arr = r.sharedMaterials;
                        if (_slot < arr.Length) { arr[_slot] = mat; r.sharedMaterials = arr; }
                    }
                    else
                    {
                        var arr = r.materials; // instance
                        if (_slot < arr.Length) { arr[_slot] = mat; r.materials = arr; }
                    }
                }
            }
        }
    }
}
