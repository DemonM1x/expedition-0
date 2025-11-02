using UnityEngine;

namespace Expedition0.Throwaway
{
    public class JetpackPlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float initialSpeed = 1.0f;
        public float rotationSpeed = 2.0f;
        public float acceleration = 0.5f;
    
        private float currentSpeed;
        private Rigidbody rb;

        void Start()
        {
            currentSpeed = initialSpeed;
            rb = GetComponent<Rigidbody>();
        
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
        
            // Set up zero gravity physics
            rb.useGravity = false;
            rb.linearDamping = 0;
            rb.angularDamping = 0;
        
            // Start with initial velocity in forward direction
            rb.linearVelocity = transform.forward * initialSpeed;
        }

        void Update()
        {
            HandleRotation();
            HandleSpeedChange();
            UpdateMovement();
        }

        private void HandleRotation()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
        
            // Create rotation vectors
            Vector3 rotation = new Vector3(-vertical, horizontal, 0) * rotationSpeed;
        
            // Apply rotation
            transform.Rotate(rotation);
        
            // Maintain upright orientation (optional - prevents unwanted rolling)
            // transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }

        private void HandleSpeedChange()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                currentSpeed -= acceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(0, currentSpeed); // Prevent negative speed
            }
        
            if (Input.GetKey(KeyCode.E))
            {
                currentSpeed += acceleration * Time.deltaTime;
            }
        }

        private void UpdateMovement()
        {
            // Maintain velocity in the current forward direction with current speed
            rb.linearVelocity = transform.forward * currentSpeed;
        }
    }
}