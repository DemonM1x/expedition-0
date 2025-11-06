using UnityEngine;
using TMPro;

namespace Expedition0.Tasks
{
    // Отображение слота значения на сцене
    public class ValueSlotView : MonoBehaviour
    {
        [Header("Text")] public TMP_Text valueLabel;

        [Header("Interaction")] public Collider interactableCollider;

        public Trit? CurrentValue { get; private set; }
        public bool IsLocked { get; private set; }

        public void ApplyValue(Trit? value, bool isLocked)
        {
            CurrentValue = value;
            IsLocked = isLocked;
            UpdateLabel();
            UpdateInteractable();
        }

        private void UpdateLabel()
        {
            if (valueLabel == null) return;
            if (!CurrentValue.HasValue)
            {
                valueLabel.text = string.Empty;
                return;
            }
            var n = CurrentValue.Value.ToInt();
            valueLabel.text = n.ToString();
        }

        private void UpdateInteractable()
        {
            if (interactableCollider != null) interactableCollider.enabled = !IsLocked;
        }
    }
}
