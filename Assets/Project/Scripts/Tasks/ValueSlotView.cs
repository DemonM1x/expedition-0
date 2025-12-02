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
                xrInteractable.selectExited.AddListener(OnSelectExited);
                xrInteractable.activated.AddListener(OnActivated);
                xrInteractable.hoverEntered.AddListener(OnHoverEntered);
                xrInteractable.hoverExited.AddListener(OnHoverExited);
                Debug.Log("XR listeners added");
            }
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            Debug.Log($"XR Hover ENTERED by: {args.interactorObject.transform.name}");
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            Debug.Log($"XR Hover EXITED by: {args.interactorObject.transform.name}");
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
            Debug.Log($"XR SELECT ENTERED by: {args.interactorObject.transform.name}");
            OnClick();
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            Debug.Log($"XR SELECT EXITED by: {args.interactorObject.transform.name}");
        }

        private void OnActivated(ActivateEventArgs args)
        {
            Debug.Log($"XR ACTIVATED by: {args.interactorObject.transform.name}");
            OnClick(); // Альтернативный способ взаимодействия
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
            if (node == null)
            {
                Debug.LogError("ValueSlotView: Trying to bind null node!");
                return;
            }
            
            boundNode = node;
            Debug.Log($"ValueSlotView: Successfully bound to AST node with value {node.CurrentValue}, locked: {node.IsLocked}");
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
            Debug.Log("ValueSlotView: Cycling value");
            if (IsLocked) 
            {
                Debug.Log("ValueSlotView: Cannot cycle - value is locked");
                return;
            }

            Trit? previousValue = CurrentValue;

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

            Debug.Log($"ValueSlotView: Changed value from {previousValue} to {CurrentValue}");

            // Обновляем AST узел
            if (boundNode != null && CurrentValue.HasValue)
            {
                try
                {
                    boundNode.SetValue(CurrentValue.Value);
                    Debug.Log($"ValueSlotView: Successfully updated AST node with value {CurrentValue.Value}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"ValueSlotView: Failed to update AST node: {e.Message}");
                    // Возвращаем предыдущее значение при ошибке
                    CurrentValue = previousValue;
                }
            }
            else
            {
                if (boundNode == null)
                    Debug.LogWarning("ValueSlotView: boundNode is null - AST not updated!");
                if (!CurrentValue.HasValue)
                    Debug.LogWarning("ValueSlotView: CurrentValue is null - AST not updated!");
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

        /// <summary>
        /// Проверяет синхронизацию между UI и AST узлом
        /// </summary>
        public bool IsInSync()
        {
            if (boundNode == null) return false;
            return CurrentValue == boundNode.CurrentValue && IsLocked == boundNode.IsLocked;
        }

        /// <summary>
        /// Принудительно синхронизирует UI с AST узлом
        /// </summary>
        public void SyncWithAST()
        {
            if (boundNode != null)
            {
                ApplyValue(boundNode.CurrentValue, boundNode.IsLocked);
                Debug.Log($"ValueSlotView: Synced with AST - Value: {boundNode.CurrentValue}, Locked: {boundNode.IsLocked}");
            }
        }

        // Методы для тестирования в инспекторе
        [ContextMenu("Test Cycle Value")]
        public void TestCycleValue()
        {
            CycleValue();
        }

        [ContextMenu("Test Set False")]
        public void TestSetFalse()
        {
            ApplyValue(Trit.False, false);
        }

        [ContextMenu("Test Set Neutral")]
        public void TestSetNeutral()
        {
            ApplyValue(Trit.Neutral, false);
        }

        [ContextMenu("Test Set True")]
        public void TestSetTrue()
        {
            ApplyValue(Trit.True, false);
        }

        [ContextMenu("Check AST Sync")]
        public void TestCheckSync()
        {
            bool inSync = IsInSync();
            Debug.Log($"ValueSlotView: AST Sync Status: {inSync}");
            if (boundNode != null)
            {
                Debug.Log($"  UI: Value={CurrentValue}, Locked={IsLocked}");
                Debug.Log($"  AST: Value={boundNode.CurrentValue}, Locked={boundNode.IsLocked}");
            }
            else
            {
                Debug.Log("  boundNode is null!");
            }
        }

        [ContextMenu("Force Sync with AST")]
        public void TestSyncWithAST()
        {
            SyncWithAST();
        }
    }
}
