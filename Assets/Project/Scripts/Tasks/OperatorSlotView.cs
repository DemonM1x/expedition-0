using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Expedition0.Tasks
{
    // Отображение слота оператора на сцене
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
    public class OperatorSlotView : MonoBehaviour
    {
        [Header("Visuals")] 
        public Image digitImage;
        public Sprite notSprite;
        public Sprite andSprite;
        public Sprite orSprite;
        public Sprite xorSprite;
        public Sprite implySprite;

        [Header("Interaction")] 
        public Collider interactableCollider;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable xrInteractable;
        private OperatorSlotNode boundNode;

        public Operator? CurrentOperator { get; private set; }
        public bool IsLocked { get; private set; }

        private void Awake()
        {
            xrInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            Debug.Log($"OperatorSlotView Awake: XRInteractable found = {xrInteractable != null}");
            
            if (xrInteractable != null)
            {
                xrInteractable.selectEntered.AddListener(OnInteractableSelected);
                xrInteractable.selectExited.AddListener(OnSelectExited);
                xrInteractable.activated.AddListener(OnActivated);
                xrInteractable.hoverEntered.AddListener(OnHoverEntered);
                xrInteractable.hoverExited.AddListener(OnHoverExited);
                Debug.Log("OperatorSlotView: XR listeners added");
            }
        }

        private void OnDestroy()
        {
            if (xrInteractable != null)
            {
                xrInteractable.selectEntered.RemoveListener(OnInteractableSelected);
                xrInteractable.selectExited.RemoveListener(OnSelectExited);
                xrInteractable.activated.RemoveListener(OnActivated);
                xrInteractable.hoverEntered.RemoveListener(OnHoverEntered);
                xrInteractable.hoverExited.RemoveListener(OnHoverExited);
            }
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            Debug.Log($"OperatorSlotView: XR Hover ENTERED by: {args.interactorObject.transform.name}");
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            Debug.Log($"OperatorSlotView: XR Hover EXITED by: {args.interactorObject.transform.name}");
        }

        private void OnInteractableSelected(SelectEnterEventArgs args)
        {
            Debug.Log($"OperatorSlotView: XR SELECT ENTERED by: {args.interactorObject.transform.name}");
            OnClick();
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            Debug.Log($"OperatorSlotView: XR SELECT EXITED by: {args.interactorObject.transform.name}");
        }

        private void OnActivated(ActivateEventArgs args)
        {
            Debug.Log($"OperatorSlotView: XR ACTIVATED by: {args.interactorObject.transform.name}");
            OnClick();
        }

        // Добавляем поддержку обычного клика мышью для тестирования
        private void OnMouseDown()
        {
            Debug.Log("OperatorSlotView: Mouse click detected");
            OnClick();
        }

        public void OnClick()
        {
            Debug.Log($"OperatorSlotView: OnClick called - IsLocked: {IsLocked}");
            if (!IsLocked)
            {
                CycleOperator();
            }
            else
            {
                Debug.Log("OperatorSlotView: Click ignored - slot is locked");
            }
        }

        public void BindNode(OperatorSlotNode node)
        {
            if (node == null)
            {
                Debug.LogError("OperatorSlotView: Trying to bind null node!");
                return;
            }
            
            boundNode = node;
            Debug.Log($"OperatorSlotView: Successfully bound to AST node with operator {node.CurrentOperator}, locked: {node.IsLocked}");
            ApplyOperator(node.CurrentOperator, node.IsLocked);
        }

        public void ApplyOperator(Operator? op, bool isLocked)
        {
            CurrentOperator = op;
            IsLocked = isLocked;
            UpdateVisuals();
            UpdateInteractable();
        }

        private void CycleOperator()
        {
            Debug.Log("OperatorSlotView: Cycling operator");
            if (IsLocked) 
            {
                Debug.Log("OperatorSlotView: Cannot cycle - operator is locked");
                return;
            }

            // Определяем доступные операторы для переключения
            Operator[] availableOperators = { Operator.NOT, Operator.AND, Operator.OR, Operator.XOR, Operator.IMPLY};
            
            Operator previousOperator = CurrentOperator ?? Operator.NOT;
            
            if (!CurrentOperator.HasValue)
            {
                CurrentOperator = availableOperators[0];
            }
            else
            {
                // Находим текущий индекс и переходим к следующему
                int currentIndex = System.Array.IndexOf(availableOperators, CurrentOperator.Value);
                int nextIndex = (currentIndex + 1) % availableOperators.Length;
                CurrentOperator = availableOperators[nextIndex];
            }

            Debug.Log($"OperatorSlotView: Changed operator from {previousOperator} to {CurrentOperator}");

            // Обновляем AST узел
            if (boundNode != null && CurrentOperator.HasValue)
            {
                try
                {
                    boundNode.SetOperator(CurrentOperator.Value);
                    Debug.Log($"OperatorSlotView: Successfully updated AST node with operator {CurrentOperator.Value}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"OperatorSlotView: Failed to update AST node: {e.Message}");
                    // Возвращаем предыдущее значение при ошибке
                    CurrentOperator = previousOperator;
                }
            }
            else
            {
                if (boundNode == null)
                    Debug.LogWarning("OperatorSlotView: boundNode is null - AST not updated!");
                if (!CurrentOperator.HasValue)
                    Debug.LogWarning("OperatorSlotView: CurrentOperator is null - AST not updated!");
            }

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (digitImage == null)
            {
                return;
            }

            if (!CurrentOperator.HasValue)
            {
                digitImage.sprite = null;
                digitImage.enabled = false;
                return;
            }

            Sprite spriteToAssign = null;
            switch (CurrentOperator.Value)
            {
                case Operator.NOT:
                    spriteToAssign = notSprite;
                    break;
                case Operator.AND:
                    spriteToAssign = andSprite;
                    break;
                case Operator.OR:
                    spriteToAssign = orSprite;
                    break;
                case Operator.XOR:
                    spriteToAssign = xorSprite;
                    break;
                case Operator.IMPLY:
                    spriteToAssign = implySprite;
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
            return CurrentOperator == boundNode.CurrentOperator && IsLocked == boundNode.IsLocked;
        }

        /// <summary>
        /// Принудительно синхронизирует UI с AST узлом
        /// </summary>
        public void SyncWithAST()
        {
            if (boundNode != null)
            {
                ApplyOperator(boundNode.CurrentOperator, boundNode.IsLocked);
                Debug.Log($"OperatorSlotView: Synced with AST - Operator: {boundNode.CurrentOperator}, Locked: {boundNode.IsLocked}");
            }
        }

        // Методы для тестирования в инспекторе
        [ContextMenu("Test Cycle Operator")]
        public void TestCycleOperator()
        {
            CycleOperator();
        }

        [ContextMenu("Test Set NOT")]
        public void TestSetNOT()
        {
            ApplyOperator(Operator.NOT, false);
        }

        [ContextMenu("Test Set AND")]
        public void TestSetAND()
        {
            ApplyOperator(Operator.AND, false);
        }

        [ContextMenu("Check AST Sync")]
        public void TestCheckSync()
        {
            bool inSync = IsInSync();
            Debug.Log($"OperatorSlotView: AST Sync Status: {inSync}");
            if (boundNode != null)
            {
                Debug.Log($"  UI: Operator={CurrentOperator}, Locked={IsLocked}");
                Debug.Log($"  AST: Operator={boundNode.CurrentOperator}, Locked={boundNode.IsLocked}");
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


