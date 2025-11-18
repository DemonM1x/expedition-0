using System.Collections.Generic;
using UnityEngine;

namespace Expedition0.Environment.Fire
{
    public class NearestFireLights : MonoBehaviour
    {
        public Camera xrCamera;
        [Header("Selection")]
        public int k = 3;                      // max enabled at once
        public float maxDist = 6f;             // consider lights within this radius
        public float moveThreshold = 0.35f;    // recompute when camera moved this far
        public float refreshInterval = 0.5f;   // fallback in seconds

        [Header("Animation")]
        public float fadeInRate = 6f;          // intensity units per second
        public float fadeOutRate = 4f;
        public float minEnable = 0.02f;        // below this, disable the light

        [Header("Optional flicker")]
        public float flickerAmp = 0.12f;       // 0 = off
        public float flickerHz = 6f;

        public static NearestFireLights Instance { get; private set; }
        
        private class State
        {
            public Light L;
            public float baseI;    // original intensity for this light
            public float current;  // current animated intensity
            public float target;   // target intensity (0 or baseI)
            public float seed;     // per-light random for flicker
        }

        private readonly List<State> _states = new();          // all lights
        private readonly Dictionary<Light, int> _index = new(); // Light -> index in _states
        private readonly List<(int idx, float d2)> _candidates = new(); // scratch
        private readonly HashSet<int> _enabledIdx = new();      // scratch

        private Vector3 _lastPos;
        private float _t;

        void Start()
        {
            if (!xrCamera) xrCamera = Camera.main;
            // Rebuild();
            // _lastPos = xrCamera ? xrCamera.transform.position : Vector3.zero;
            _t = refreshInterval;
            _lastPos = xrCamera ? xrCamera.transform.position + new Vector3(moveThreshold*2f,0,0)
                : Vector3.zero;
        }
        
        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        /// <summary>Call this whenever new fires/lights are created at runtime.</summary>
        public void Rebuild()
        {
            _states.Clear();
            _index.Clear();

            foreach (var fire in FindObjectsByType<Fire>(FindObjectsSortMode.None))
            {
                foreach (var L in fire.Lights)
                {
                    if (!L) continue;
                    var st = new State
                    {
                        L = L,
                        baseI = L.intensity,
                        current = 0f,
                        target = 0f,
                        seed = Random.value * 10f
                    };
                    L.intensity = 0f;
                    L.enabled = false;
                    _index.Add(L, _states.Count);
                    _states.Add(st);
                }
            }
        }
        
        // add a light once (from Fire.Ignite)
        public void RegisterLight(Light l)
        {
            if (!l || _index.ContainsKey(l)) return;
            var st = new State { L=l, baseI = Mathf.Max(0.001f, l.intensity), current = 0f, target = 0f, seed = Random.value*10f };
            l.intensity = 0f; l.enabled = false;
            _index[l] = _states.Count;
            _states.Add(st);
        }

        // remove or null a light (from Fire.Extinguish/OnDestroy)
        public void UnregisterLight(Light l)
        {
            if (!l) return;
            if (_index.TryGetValue(l, out int i))
            {
                _states[i].L = null;   // keep index stable; no list compaction
                _index.Remove(l);
            }
        }

        void LateUpdate()
        {
            if (!xrCamera) return;

            _t += Time.deltaTime;
            var p = xrCamera.transform.position;

            // Recompute selection only if player moved enough or the fallback elapsed
            if ((p - _lastPos).sqrMagnitude >= moveThreshold * moveThreshold || _t >= refreshInterval)
            {
                _lastPos = p;
                _t = 0f;

                // Build candidate list (indices + squared distance)
                _candidates.Clear();
                float r2 = maxDist * maxDist;
                for (int i = 0; i < _states.Count; i++)
                {
                    var L = _states[i].L;
                    if (!L || !L.gameObject.activeInHierarchy) continue;
                    float d2 = (L.transform.position - p).sqrMagnitude;
                    if (d2 <= r2) _candidates.Add((i, d2));
                }

                // Find k nearest; for small k this linear partial selection is cheaper than sorting all
                _enabledIdx.Clear();
                int want = Mathf.Min(k, _candidates.Count);
                for (int m = 0; m < want; m++)
                {
                    int best = m;
                    for (int j = m + 1; j < _candidates.Count; j++)
                        if (_candidates[j].d2 < _candidates[best].d2) best = j;

                    // swap best into position m
                    (_candidates[m], _candidates[best]) = (_candidates[best], _candidates[m]);

                    _enabledIdx.Add(_candidates[m].idx);
                }

                // Set targets: enabled -> baseI, others -> 0
                for (int i = 0; i < _states.Count; i++)
                {
                    var st = _states[i];
                    st.target = _enabledIdx.Contains(i) ? st.baseI : 0f;
                    _states[i] = st;
                }
            }

            // Animate every frame (cheap)
            float tNow = Time.time;
            for (int i = 0; i < _states.Count; i++)
            {
                var st = _states[i];
                if (!st.L) continue;

                float rate = (st.target > st.current) ? fadeInRate : fadeOutRate;
                st.current = Mathf.MoveTowards(st.current, st.target, rate * Time.deltaTime);

                if (st.current > minEnable)
                {
                    if (flickerAmp > 0f)
                    {
                        float f = 1f + flickerAmp * (Mathf.PerlinNoise(st.seed, tNow * flickerHz) - 0.5f) * 2f;
                        st.L.intensity = st.current * f;
                    }
                    else
                    {
                        st.L.intensity = st.current;
                    }
                    st.L.enabled = true;
                }
                else
                {
                    st.L.enabled = false;
                    st.L.intensity = 0f;
                }

                _states[i] = st;
            }
        }
    }
}
