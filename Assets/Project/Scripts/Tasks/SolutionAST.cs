using UnityEngine;
using Expedition0.Tasks;

namespace Expedition0.Tasks
{
    public static class SolutionAST
    {
        public static Trit Solution(ASTNode root)
        {
            return root.Evaluate();
        }
    }
}