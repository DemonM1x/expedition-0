using System;
using UnityEngine;
using UnityEngine.Events;

namespace Expedition0.Environment
{
    public class CollisionEventCaller: MonoBehaviour
    {
        [SerializeField] protected UnityEvent<GameObject> onTriggerEntered;
        [SerializeField] protected UnityEvent<GameObject> onCollisionEntered;

        protected void OnTriggerEnter(Collider other)
        {
            var obj = other?.gameObject;
            if (obj != null) onTriggerEntered?.Invoke(obj);
        }

        protected void OnCollisionEnter(Collision collision)
        {
            var obj = collision.gameObject;
            if (obj != null) onCollisionEntered?.Invoke(obj);
        }
    }
}