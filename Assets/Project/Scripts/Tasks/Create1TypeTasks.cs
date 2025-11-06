using Expedition0.Tasks;

namespace Expedition0.Tasks
{
    // Утилиты для создания задач типа 1
    public static class Create1TypeTasks
    {
        // Пример «1 OR X = 2»:
        // оператор OR залочен, левое значение фиксировано 1 (Neutral), правое — пустое;
        // ответ задан как 2 (True)
        public static ASTTemplate CreateNeutralOrXEqualsTrue()
        {
            var tmpl = Type1TaskTemplateFactory.CreateBinary(
                predefinedOperator: Operator.OR,
                answer: Trit.True,
                lockOperator: true,
                leftLocked: Trit.Neutral,
                rightLocked: null
            );
            return tmpl;
        } 

        // Вспомогательный метод проверки соответствия левой части ответу
        public static bool Check(ASTTemplate template)
        {
            var lhs = SolutionAST.Solution(template.Root);
            return lhs == template.Answer;
        }
    }
}