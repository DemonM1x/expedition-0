using UnityEngine;
using TMPro;

namespace Expedition0.Tasks
{
    // Привязывает ASTTemplate к элементам сцены (по индексам)
    public class TaskBoardBinder : MonoBehaviour
    {
        [Header("Mapping by index")]
        public GameObject[] valueSlots;
        public GameObject[] operatorSlots;

        [Header("Optional direct TMP mapping (bypass *View)")]
        public TMP_Text[] valueLabels;
        public TMP_Text[] operatorLabels;

        [Header("Answer (right side)")]
        public TMP_Text answerLabel;

        public void Bind(ASTTemplate template)
        { 
            // Значения
            var vs = template.ValueSlots;
            for (int i = 0; i < valueSlots.Length; i++)
            {
                if (i >= vs.Count) break;
                var slotNode = vs[i];
                var go = valueSlots[i];
                if (go != null)
                {
                    var view = go.GetComponentInChildren<ValueSlotView>();
                    if (view != null) view.BindNode(slotNode);
                }

                if (valueLabels != null && i < valueLabels.Length && valueLabels[i] != null)
                {
                    var label = valueLabels[i];
                    label.text = slotNode.CurrentValue.HasValue ? ((int)slotNode.CurrentValue.Value).ToString() : string.Empty;
                }
            }

            // Операторы
            var os = template.OperatorSlots;
            for (int i = 0; i < operatorSlots.Length; i++)
            {
                if (i >= os.Count) break;
                var slotNode = os[i];
                var go = operatorSlots[i];
                if (go != null)
                {
                    var view = go.GetComponentInChildren<OperatorSlotView>();
                    if (view != null && slotNode.CurrentOperator.HasValue)
                        view.ApplyOperator(slotNode.CurrentOperator.Value, slotNode.IsLocked);
                }

                if (operatorLabels != null && i < operatorLabels.Length && operatorLabels[i] != null)
                {
                    var label = operatorLabels[i];
                    label.text = slotNode.CurrentOperator.HasValue ? OperatorToText(slotNode.CurrentOperator.Value) : string.Empty;
                }
            }

            // Ответ (правая часть)
            if (answerLabel != null)
            {
                answerLabel.text = ((int)template.Answer).ToString();
            }
        }

        private string OperatorToText(Operator op)
        {
            switch (op)
            {
                case Operator.NOT: return "NOT";
                case Operator.AND: return "AND";
                case Operator.OR: return "OR";
                case Operator.XOR: return "XOR";
                case Operator.IMPLY: return "IMPLY";
                default: return string.Empty;
            }
        }
    }
}


