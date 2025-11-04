using System.Collections.Generic;
using UnityEngine;

namespace Expedition0.Throwaway
{
    [ExecuteInEditMode]
    public class LampColorController : MonoBehaviour
    {
        [SerializeField] private Color emissionColor = Color.white;
        [SerializeField] private float emissionIntensity = 2f;
        [SerializeField] private bool updateLights = true;
        [SerializeField] private Color lightColor = Color.white;
        [SerializeField] private float lightIntensity = 1f;
    
        private MaterialPropertyBlock _propBlock;
        private List<Renderer> _renderers;
        private List<Light> _lightSources;
        private bool _isInitialized = false;

        private void OnEnable()
        {
            Initialize();
            UpdateAll();
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            _propBlock = new MaterialPropertyBlock();
            _renderers = new List<Renderer>();
            _lightSources = new List<Light>();
            
            // Get all renderers on this object and children
            GetComponentsInChildren(_renderers);
            
            // Get all light components
            if (updateLights)
            {
                GetComponentsInChildren(_lightSources);
            }

            _isInitialized = true;
        }

        private void OnValidate()
        {
            // This gets called when values change in the inspector
            if (_isInitialized)
            {
                UpdateAll();
            }
            else if (isActiveAndEnabled)
            {
                Initialize();
                UpdateAll();
            }
        }

        void Start()
        {
            Initialize();
            UpdateAll();
        }

        private void UpdateAll()
        {
            UpdateEmissionColor();
            if (updateLights)
            {
                UpdateLights();
            }
        }

        private void UpdateEmissionColor()
        {
            if (_renderers == null || _renderers.Count == 0) return;

            Color finalColor = emissionColor * emissionIntensity;
            
            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;
                
                renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetColor("_EmissionColor", finalColor);
                renderer.SetPropertyBlock(_propBlock);
            }
        }

        private void UpdateLights()
        {
            if (_lightSources == null || _lightSources.Count == 0) return;

            foreach (var lightSource in _lightSources)
            {
                if (lightSource == null) continue;
                
                lightSource.color = lightColor;
                lightSource.intensity = lightIntensity;
            }
        }

        public void ChangeEmissionColor(Color newColor, float intensity = -1f)
        {
            emissionColor = newColor;
            if (intensity >= 0)
            {
                emissionIntensity = intensity;
            }
            UpdateAll();
        }

        public void ChangeLightColor(Color newLightColor, float newLightIntensity = -1f)
        {
            lightColor = newLightColor;
            if (newLightIntensity >= 0)
            {
                lightIntensity = newLightIntensity;
            }
            UpdateAll();
        }

        // Editor helper method to force refresh
        [ContextMenu("Refresh Lamp Colors")]
        private void RefreshLampColors()
        {
            Initialize();
            UpdateAll();
        }

        // Method to get current state
        public void GetCurrentColors(out Color currentEmissionColor, out float currentEmissionIntensity, 
                                   out Color currentLightColor, out float currentLightIntensity)
        {
            currentEmissionColor = emissionColor;
            currentEmissionIntensity = emissionIntensity;
            currentLightColor = lightColor;
            currentLightIntensity = lightIntensity;
        }
    }
}