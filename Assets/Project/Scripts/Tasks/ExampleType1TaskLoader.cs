using UnityEngine;

namespace Expedition0.Tasks
{
    // Простой загрузчик: выбирает функцию-шаблон и вызывает Bind при загрузке сцены
    public class ExampleType1TaskLoader : MonoBehaviour
    {
        public TaskBoardBinder binder;

        // Временный переключатель: использовать пример 1 OR X = 2
        public bool useNeutralOrXEqualsTrue = true;

        public ASTTemplate template;

        private void Start()
        {
            if (binder == null)
            {
                binder = GetComponent<TaskBoardBinder>();
            }
            if (binder == null) return;

            if (useNeutralOrXEqualsTrue)
            {
                template = Create1TypeTasks.CreateNeutralOrXEqualsTrue();
            }
            else
            {
                // Пример альтернативы: один бинарный оператор с вашими параметрами
                template = Type1TaskTemplateFactory.CreateBinary(
                    predefinedOperator: Operator.OR,
                    answer: Trit.True,
                    lockOperator: true,
                    leftLocked: Trit.Neutral,
                    rightLocked: null
                );
            }

            binder.Bind(template);
        }

        // Публичный метод для получения template
        public ASTTemplate GetTemplate()
        {
            return template;
        }

        // Публичный метод для получения корневого узла AST
        public ASTNode GetRootNode()
        {
            return template?.Root;
        }
    }
}


