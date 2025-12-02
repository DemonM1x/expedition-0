 namespace Expedition0.Tasks
{             
    // Фабрика шаблонов для заданий типа 1: операции заранее определены и могут быть залочены
    public static class Type1TaskTemplateFactory
    {
         // ПУБЛИЧНЫЕ МЕТОДЫ: только одна бинарная операция и две ассоциативности с двумя операциями
        public static ASTTemplate CreateBinary(Operator predefinedOperator, Trit answer, bool lockOperator = true, Trit? leftLocked = null, Trit? rightLocked = null)
        {
            var left = new ValueSlotNode();
            var right = new ValueSlotNode();

            if (leftLocked.HasValue) left.LockValue(leftLocked.Value);
            if (rightLocked.HasValue) right.LockValue(rightLocked.Value);

            var op = new OperatorSlotNode(left, right);
            if (lockOperator) op.LockOperator(predefinedOperator); else op.SetOperator(predefinedOperator);

            return new ASTTemplate(op, answer);
        }

        public static ASTTemplate CreateTwoOpsLeftAssoc(Operator op1, Operator op2, Trit answer, bool lockOperators = true, Trit? a = null, Trit? b = null, Trit? c = null)
        {
            var v1 = new ValueSlotNode(); if (a.HasValue) v1.LockValue(a.Value);
            var v2 = new ValueSlotNode(); if (b.HasValue) v2.LockValue(b.Value);
            var v3 = new ValueSlotNode(); if (c.HasValue) v3.LockValue(c.Value);

            var inner = new OperatorSlotNode(v1, v2);
            if (lockOperators) inner.LockOperator(op1); else inner.SetOperator(op1);

            var root = new OperatorSlotNode(inner, v3);
            if (lockOperators) root.LockOperator(op2); else root.SetOperator(op2);

            return new ASTTemplate(root, answer);
        }

        public static ASTTemplate CreateTwoOpsRightAssoc(Operator op1, Operator op2, Trit answer, bool lockOperators = true, Trit? a = null, Trit? b = null, Trit? c = null)
        {
            var v1 = new ValueSlotNode(); if (a.HasValue) v1.LockValue(a.Value);
            var v2 = new ValueSlotNode(); if (b.HasValue) v2.LockValue(b.Value);
            var v3 = new ValueSlotNode(); if (c.HasValue) v3.LockValue(c.Value);

            var inner = new OperatorSlotNode(v2, v3);
            if (lockOperators) inner.LockOperator(op2); else inner.SetOperator(op2);

            var root = new OperatorSlotNode(v1, inner);
            if (lockOperators) root.LockOperator(op1); else root.SetOperator(op1);

            return new ASTTemplate(root, answer);
        }
    }
}


