using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

namespace Expedition0.Environment
{
    public enum ElevatorLevel
    {
        Lobby,
        Greenhouse,
        OuterSkeleton,
        MachineHall,
        Death,
        SpaceCombat,
        IllusionRoom,
        Victory
    }

    [Serializable]
    public class ElevatorLevelIconEntry
    {
        public ElevatorLevel level;
        [Header("Unlocked")]
        public Sprite leftUnlockedSprite;
        public Sprite rightUnlockedSprite;
        [Header("Locked")]
        public Sprite leftLockedSprite;
        public Sprite rightLockedSprite;
    }
    
    [RequireComponent(typeof(Animator))]
    public class ElevatorController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string openPropertyName = "IsOpen";
        [SerializeField] private UnityEvent<bool> onSuccessfulDoorToggle;

        [Header("Elevator State")]
        [SerializeField] private bool locked = false;
        [SerializeField] private ElevatorLevel elevatorLevel;

        [Header("Elevator Status Icon")]
        [SerializeField] private SpriteRenderer leftSpriteRenderer;
        [SerializeField] private SpriteRenderer rightSpriteRenderer;
        [SerializeField] private Material unlockedIconMaterial;
        [SerializeField] private Material lockedIconMaterial;
        [SerializeField] private List<ElevatorLevelIconEntry> icons;
        
        
        private int _openPropertyHash = -1;

        /// <summary>Gets/sets the Animator bool (IsOpen by default).</summary>
        public bool Open
        {
            get => animator.GetBool(_openPropertyHash);
            set => animator.SetBool(_openPropertyHash, value);
        }

        public bool Locked => locked;
        
        void Reset()
        {
            animator = GetComponent<Animator>();
        }

        void Awake()
        {
            if (!animator) animator = GetComponent<Animator>(); 
            _openPropertyHash = Animator.StringToHash(openPropertyName);
        }

        /// <summary>Convenience method callable from UI/Events: sets IsOpen.</summary>
        public void SetOpen(bool open)
        {
            if (Open == open) return;
            
            if (!Locked)
            {
                Open = open;
                onSuccessfulDoorToggle?.Invoke(open);
            }
        }
        
        public void SetLocked(bool newLocked)
        {
            bool prev = locked;
            locked = newLocked;
            if (prev != newLocked)
            {
                UpdateVisualizationColor();
                UpdateVisualizationIcons();
            }
        }

        /// <summary>Toggles the door state (optional helper).</summary>
        public void ToggleOpen() => SetOpen(!Open);

        /// <summary>Explicit helpers (optional).</summary>
        public void OpenDoor()  => SetOpen(true);
        public void CloseDoor() => SetOpen(false);
        
        private void UpdateVisualizationIcons()
        {
            if (leftSpriteRenderer != null)
            {
                var icon = icons.FirstOrDefault(
                    entry => (entry.level == elevatorLevel)
                );
                if (icon != null)
                {
                    leftSpriteRenderer.sprite = locked ? icon.leftLockedSprite : icon.leftUnlockedSprite;
                    rightSpriteRenderer.sprite = locked ? icon.rightLockedSprite : icon.rightUnlockedSprite;
                }
            }
        }
        private void UpdateVisualizationColor()
        {
            var material = locked ? lockedIconMaterial : unlockedIconMaterial;
            leftSpriteRenderer.material = material;
            rightSpriteRenderer.material = material;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !Locked) OpenDoor();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && !Locked) CloseDoor();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            // Update visualization immediately when values change in inspector
            UpdateVisualizationIcons();
            UpdateVisualizationColor();
        
            // Ensure animator reference is maintained
            if (animator == null)
                animator = GetComponent<Animator>();
        }
#endif
    }
}
