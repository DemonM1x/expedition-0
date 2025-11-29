using UnityEngine;

namespace Expedition0.Tasks
{
    public enum TaskType
    {
        NeutralOrXEqualsTrue,           // 1 OR X = 2 (оператор заблокирован)
        NeutralOrXEqualsTrueUnlocked,   // 1 OR X = 2 (оператор разблокирован)
        CustomBinary                    // Пользовательский бинарный оператор
    }

    // Простой загрузчик: выбирает функцию-шаблон и вызывает Bind при загрузке сцены
    public class ExampleType1TaskLoader : MonoBehaviour
    {
        [Header("Task Configuration")]
        public TaskBoardBinder binder;
        
        [Header("Task Selection")]
        public TaskType selectedTaskType = TaskType.NeutralOrXEqualsTrue;
        
        [Header("Custom Binary Task Settings (only for CustomBinary type)")]
        public Operator customOperator = Operator.AND;
        public Trit customAnswer = Trit.True;
        public bool lockCustomOperator = true;
        public bool lockLeftValue = false;
        public bool lockRightValue = false;
        
        [Header("Custom Values (only used when locked)")]
        public Trit customLeftValue = Trit.False;
        public Trit customRightValue = Trit.False;

        public ASTTemplate template;

        private void Start()
        {
            if (binder == null)
            {
                binder = GetComponent<TaskBoardBinder>();
            }
            if (binder == null) return;

            template = CreateTaskByType(selectedTaskType);
            
            if (template != null)
            {
                binder.Bind(template);
                Debug.Log($"ExampleType1TaskLoader: Loaded task type: {selectedTaskType}");
            }
            else
            {
                Debug.LogError("ExampleType1TaskLoader: Failed to create task template!");
            }
        }

        private ASTTemplate CreateTaskByType(TaskType taskType)
        {
            switch (taskType)
            {
                case TaskType.NeutralOrXEqualsTrue:
                    return Create1TypeTasks.CreateNeutralOrXEqualsTrue();
                
                case TaskType.NeutralOrXEqualsTrueUnlocked:
                    return CreateNeutralOrXEqualsTrueUnlocked();
                
                case TaskType.CustomBinary:
                    return CreateCustomBinaryTask();
                
                default:
                    Debug.LogError($"ExampleType1TaskLoader: Unknown task type: {taskType}");
                    return null;
            }
        }

        private ASTTemplate CreateNeutralOrXEqualsTrueUnlocked()
        {
            // Аналогично NeutralOrXEqualsTrue, но с разблокированным оператором
            return Type1TaskTemplateFactory.CreateBinary(
                predefinedOperator: Operator.OR,
                answer: Trit.True,
                lockOperator: false,        // Оператор разблокирован!
                leftLocked: Trit.Neutral,   // Левое значение заблокировано на 1 (Neutral)
                rightLocked: null           // Правое значение свободно для ввода
            );
        }

        private ASTTemplate CreateCustomBinaryTask()
        {
            Trit? leftValue = lockLeftValue ? customLeftValue : null;
            Trit? rightValue = lockRightValue ? customRightValue : null;
            
            return Type1TaskTemplateFactory.CreateBinary(
                predefinedOperator: customOperator,
                answer: customAnswer,
                lockOperator: lockCustomOperator,
                leftLocked: leftValue,
                rightLocked: rightValue
            );
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


        public void ChangeTaskType(TaskType newTaskType)
        {
            selectedTaskType = newTaskType;
            ReloadTask();
        }


        public void ReloadTask()
        {
            template = CreateTaskByType(selectedTaskType);
            
            if (template != null && binder != null)
            {
                binder.Bind(template);
                Debug.Log($"ExampleType1TaskLoader: Reloaded task type: {selectedTaskType}");
            }
            else
            {
                Debug.LogError("ExampleType1TaskLoader: Failed to reload task template!");
            }
        }


        public void LoadCustomTask(Operator op, Trit answer, bool lockOp, Trit? leftVal = null, Trit? rightVal = null)
        {
            customOperator = op;
            customAnswer = answer;
            lockCustomOperator = lockOp;
            lockLeftValue = leftVal.HasValue;
            lockRightValue = rightVal.HasValue;
            
            if (leftVal.HasValue)
                customLeftValue = leftVal.Value;
            if (rightVal.HasValue)
                customRightValue = rightVal.Value;
            
            ChangeTaskType(TaskType.CustomBinary);
        }

        public string GetTaskInfo()
        {
            if (template == null) return "No task loaded";
            
            string info = $"Task Type: {selectedTaskType}\n";
            info += $"Answer: {template.Answer}\n";
            info += $"Value Slots: {template.ValueSlots.Count}\n";
            info += $"Operator Slots: {template.OperatorSlots.Count}\n";
            
            // Добавляем информацию о настройках для CustomBinary
            if (selectedTaskType == TaskType.CustomBinary)
            {
                info += $"\nCustom Settings:\n";
                info += $"  Operator: {customOperator} (Locked: {lockCustomOperator})\n";
                info += $"  Left Value: {(lockLeftValue ? customLeftValue.ToString() : "Unlocked")}\n";
                info += $"  Right Value: {(lockRightValue ? customRightValue.ToString() : "Unlocked")}\n";
                info += $"  Expected Answer: {customAnswer}";
            }
            
            return info;
        }

        /// <summary>
        /// Проверяет корректность настроек задания
        /// </summary>
        public bool ValidateTaskSettings()
        {
            if (selectedTaskType == TaskType.CustomBinary)
            {
                // Для NOT оператора правое значение не должно быть заблокировано
                if (customOperator == Operator.NOT && lockRightValue)
                {
                    Debug.LogWarning("ExampleType1TaskLoader: NOT operator doesn't use right operand, but right value is locked!");
                    return false;
                }
            }
            
            return true;
        }

        [ContextMenu("Reload Current Task")]
        public void TestReloadTask()
        {
            ReloadTask();
        }

        [ContextMenu("Load NeutralOrX Task (Locked)")]
        public void TestLoadLockedTask()
        {
            ChangeTaskType(TaskType.NeutralOrXEqualsTrue);
        }

        [ContextMenu("Load NeutralOrX Task (Unlocked)")]
        public void TestLoadUnlockedTask()
        {
            ChangeTaskType(TaskType.NeutralOrXEqualsTrueUnlocked);
        }

        [ContextMenu("Load Custom AND Task")]
        public void TestLoadCustomANDTask()
        {
            LoadCustomTask(Operator.AND, Trit.True, false, Trit.True, null);
        }

        [ContextMenu("Load Custom XOR Task (Both Unlocked)")]
        public void TestLoadCustomXORTask()
        {
            LoadCustomTask(Operator.XOR, Trit.Neutral, false, null, null);
        }

        [ContextMenu("Load Custom IMPLY Task (Left Locked)")]
        public void TestLoadCustomIMPLYTask()
        {
            LoadCustomTask(Operator.IMPLY, Trit.True, true, Trit.False, null);
        }

        [ContextMenu("Load Custom NOT Task")]
        public void TestLoadCustomNOTTask()
        {
            // NOT оператор работает только с левым операндом
            LoadCustomTask(Operator.NOT, Trit.True, false, null, null);
        }

        [ContextMenu("Print Task Info")]
        public void TestPrintTaskInfo()
        {
            Debug.Log(GetTaskInfo());
        }
    }
}


