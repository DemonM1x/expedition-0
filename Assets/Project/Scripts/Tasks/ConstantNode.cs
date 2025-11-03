using UnityEngine;
namespace Expedition0.Tasks
{
public class ConstantNode : ASTNode
    {
        public Trit Value { get; set; }

        public ConstantNode(Trit value)
        {
            Value = value;
        }
        public override Trit Evaluate() => Value;
    }
}
