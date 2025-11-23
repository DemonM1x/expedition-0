using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Expedition0.Tasks
{
    // Отображение слота значения на сцене
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
    public class ValueSlotView : MonoBehaviour
    {

        [Header("Digit Sprites")]
        public Sprite digit0Sprite;
        public Sprite digit1Sprite;
        public Sprite digit2Sprite;
        public Image digitImage;

        [Header("Interaction")] public Collider interactableCollider;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable xrInteractable;
        private ValueSlotNode boundNode;

        public Trit? CurrentValue { get; private set; }
        public bool IsLocked { get; private set; }

        private void Awake()
        {
            xrInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            Debug.Log($"ValueSlotView Awake: XRInteractable found = {xrInteractable != null}");
            
            if (xrInteractable != null)
            {
                xrInteractable.selectEntered.AddListener(OnInteractableSelected);
                Debug.Log("XR Select listener added");
            }
        }

        // Добавляем поддержку обычного клика мышью для тестирования
        private void OnMouseDown()
        {
            Debug.Log("Mouse click detected");
            OnClick();
        }

        private void OnDestroy()
        {
            if (xrInteractable != null)
            {
                xrInteractable.selectEntered.RemoveListener(OnInteractableSelected);
            }
        }

        private void OnInteractableSelected(SelectEnterEventArgs args)
        {
            OnClick();
        }

        public void OnClick()
        {
            Debug.Log($"OnClick called - IsLocked: {IsLocked}");
            if (!IsLocked)
            {
                CycleValue();
            }
            else
            {
                Debug.Log("Click ignored - slot is locked");
            }
        }

        public void BindNode(ValueSlotNode node)
        {
            boundNode = node;
            ApplyValue(node.CurrentValue, node.IsLocked);
        }

        public void ApplyValue(Trit? value, bool isLocked)
        {
            CurrentValue = value;
            IsLocked = isLocked;
            UpdateLabel();
            UpdateInteractable();
        }

        private void CycleValue()
        {
            Debug.Log("pressed");
            if (IsLocked) return;

            if (!CurrentValue.HasValue)
            {
                CurrentValue = Trit.False;
            }
            else
            {
                int currentInt = CurrentValue.Value.ToInt();
                int nextInt = (currentInt + 1) % 3;
                CurrentValue = TritLogic.FromInt(nextInt);
            }

            if (boundNode != null && CurrentValue.HasValue)
            {
                boundNode.SetValue(CurrentValue.Value);
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (digitImage == null)
            {
                return;
            }

            if (!CurrentValue.HasValue)
            {
                digitImage.sprite = null;
                digitImage.enabled = false;
                return;
            }

            var digitValue = CurrentValue.Value.ToInt();
            Sprite spriteToAssign = null;

            switch (digitValue)
            {
                case 0:
                    spriteToAssign = digit0Sprite;
                    break;
                case 1:
                    spriteToAssign = digit1Sprite;
                    break;
                case 2:
                    spriteToAssign = digit2Sprite;
                    break;
            }

            digitImage.sprite = spriteToAssign;
            digitImage.enabled = spriteToAssign != null;
        }

        private void UpdateInteractable()
        {
            if (interactableCollider != null) interactableCollider.enabled = !IsLocked;
        }
    }
}
