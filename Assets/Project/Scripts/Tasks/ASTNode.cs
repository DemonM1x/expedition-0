using UnityEngine;
using System.Collections.Generic;
namespace Expedition0.Tasks
{
    public abstract class ASTNode {
        public ASTNode Parent { get; set; }
        public List<ASTNode> Children { get; set; } = new List<ASTNode>();

        public abstract Trit Evaluate();
    }

}
