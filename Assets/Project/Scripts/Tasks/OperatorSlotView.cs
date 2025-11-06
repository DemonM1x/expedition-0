using UnityEngine;

namespace Expedition0.Tasks
{
    // Отображение слота оператора на сцене
    public class OperatorSlotView : MonoBehaviour
    {
        [Header("Visuals")] public SpriteRenderer spriteRenderer;
        public Sprite notSprite;
        public Sprite andSprite;
        public Sprite orSprite;
        public Sprite xorSprite;
        public Sprite implySprite;

        [Header("Interaction")] public Collider interactableCollider;

        public Operator? CurrentOperator { get; private set; }
        public bool IsLocked { get; private set; }

        public void ApplyOperator(Operator op, bool isLocked)
        {
            CurrentOperator = op;
            IsLocked = isLocked;
            UpdateVisuals();
            UpdateInteractable();
        }

        private void UpdateVisuals()
        {
            if (spriteRenderer == null || !CurrentOperator.HasValue) return;
            switch (CurrentOperator.Value)
            {
                case Operator.NOT: spriteRenderer.sprite = notSprite; break;
                case Operator.AND: spriteRenderer.sprite = andSprite; break;
                case Operator.OR: spriteRenderer.sprite = orSprite; break;
                case Operator.XOR: spriteRenderer.sprite = xorSprite; break;
                case Operator.IMPLY: spriteRenderer.sprite = implySprite; break;
                default: break;
            }
        }

        private void UpdateInteractable()
        {
            if (interactableCollider != null) interactableCollider.enabled = !IsLocked;
        }
    }
}


