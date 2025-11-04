using UnityEngine;
namespace Expedition0.Tasks
{
public class BinaryOperatorNode : ASTNode
    {
    public ASTNode Left { get; set; }
    public ASTNode Right { get; set; }
    public Operator Operator { get; set; }
    public BinaryOperatorNode(ASTNode left, ASTNode right)
    {
        Left = left;
        Right = right;
    }
    public override Trit Evaluate() {
    switch (Operator) {
        case Operator.NOT:
            return Left.Evaluate().Not();
        case Operator.AND:
            return Left.Evaluate().And(Right.Evaluate());
        case Operator.OR:
            return Left.Evaluate().Or(Right.Evaluate());
        case Operator.XOR:
            return Left.Evaluate().Xor(Right.Evaluate());
        case Operator.IMPLY:
            return Left.Evaluate().ImplyKleene(Right.Evaluate());
        default:
            throw new System.Exception("Invalid operator");
    }
    }
    }
}