using System.Collections.Generic;
using UnityEngine;

namespace Expedition0.Environment.Fire
{
    public class FireGroup : MonoBehaviour
    {
        public List<Fire> Fires = new();

        void Awake()
        {
            if (Fires.Count == 0) Fires.AddRange(GetComponentsInChildren<Fire>(true));
        }

        public void IgniteAll()
        {
            foreach (var f in Fires) if (f) f.Ignite();
        }

        public void ExtinguishAll()
        {
            foreach (var f in Fires) if (f) f.Extinguish();
        }
    }
}