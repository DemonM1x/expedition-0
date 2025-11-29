using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Expedition0.Util;

namespace Expedition0.Tasks
{
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
    public class SolutionChecker : MonoBehaviour
    {
        [Header("Solution Settings")]
        [SerializeField] private ExampleType1TaskLoader taskLoader; // Ссылка на загрузчик задач
        private Trit answer;
        
        [Header("Material Assigners")]
        [SerializeField] private MaterialAssigner equalsAssigner; // MaterialAssigner для символа "равно"
        [SerializeField] private MaterialAssigner notEqualsAssigner; // MaterialAssigner для символа "не равно"
        
        [Header("Alternative: Direct Renderer Control")]
        [SerializeField] private Renderer targetRenderer; // Renderer модели
        [SerializeField] private int equalsSlotIndex = 0; // Индекс слота для символа "="
        [SerializeField] private int slashSlotIndex = 1; // Индекс слота для символа "/"
        
        [Header("Materials")]
        [SerializeField] private Material correctMaterial;
        [SerializeField] private Material incorrectMaterial;
        [SerializeField] private Material transparentMaterial; // Прозрачный материал для скрытия символов
        
        [Header("Error Counter")]
        [SerializeField] private int errorCount = 0;
        
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable xrInteractable;

        public int ErrorCount => errorCount;

        private void Awake()
        {
            xrInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            
            if (xrInteractable != null)
            {
                xrInteractable.selectEntered.AddListener(OnInteractableSelected);
                Debug.Log("SolutionChecker: XR Select listener added");
            }
            
            // Проверяем наличие MaterialAssigner'ов
            if (equalsAssigner == null)
            {
                Debug.LogWarning("SolutionChecker: Equals MaterialAssigner is not assigned!");
            }
            
            if (notEqualsAssigner == null)
            {
                Debug.LogWarning("SolutionChecker: Not Equals MaterialAssigner is not assigned!");
            }
        }

        private void OnDestroy()
        {
            if (xrInteractable != null)
            {
                xrInteractable.selectEntered.RemoveListener(OnInteractableSelected);
            }
        }

        private void OnInteractableSelected(SelectEnterEventArgs args)
        {
            CheckSolution();
        }

        // Альтернативный метод для тестирования мышью
        private void OnMouseDown()
        {
            Debug.Log("SolutionChecker: Mouse click detected");
            CheckSolution();
        }

        public void CheckSolution()
        {
            if (taskLoader == null)
            {
                Debug.LogError("SolutionChecker: Task loader is not assigned!");
                return;
            }

            ASTNode leftSideRoot = taskLoader.GetRootNode();
            answer = taskLoader.GetTemplate().Answer;
            if (leftSideRoot == null)
            {
                Debug.LogError("SolutionChecker: Template or root node is not available!");
                return;
            }

            try
            {
                Trit leftResult = SolutionAST.Solution(leftSideRoot);
                
                Debug.Log($"SolutionChecker: Left side = {leftResult}, Answer = {answer}");
                bool isCorrect = leftResult == answer;
                
                if (isCorrect)
                {
                    Debug.Log("SolutionChecker: Solution is CORRECT!");
                    ApplyCorrectMaterial();
                }
                else
                {
                    Debug.Log("SolutionChecker: Solution is INCORRECT!");
                    errorCount++;
                    Debug.Log($"SolutionChecker: Error count increased to {errorCount}");
                    ApplyIncorrectMaterial();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"SolutionChecker: Error evaluating solution: {e.Message}");
                errorCount++;
                ApplyIncorrectMaterial();
            }
        }

        private void ApplyCorrectMaterial()
        {
            // Способ 1: Через MaterialAssigner'ы
            if (equalsAssigner != null && notEqualsAssigner != null)
            {
                if (correctMaterial != null)
                {
                    equalsAssigner.CurrentMaterial = correctMaterial;
                    Debug.Log("SolutionChecker: Applied correct material to equals symbol (=)");
                }
                
                // Делаем символ "/" прозрачным или невидимым
                if (transparentMaterial != null)
                {
                    notEqualsAssigner.CurrentMaterial = transparentMaterial;
                    Debug.Log("SolutionChecker: Made slash symbol (/) transparent");
                }
            }
            // Способ 2: Прямое управление Renderer'ом
            else if (targetRenderer != null)
            {
                ApplyDirectRendererMaterials(true);
            }
        }

        private void ApplyIncorrectMaterial()
        {
            // Способ 1: Через MaterialAssigner'ы
            if (equalsAssigner != null && notEqualsAssigner != null)
            {
                // Для символа "не равно" оба символа "=" и "/" должны светиться красным
                if (incorrectMaterial != null)
                {
                    equalsAssigner.CurrentMaterial = incorrectMaterial;
                    notEqualsAssigner.CurrentMaterial = incorrectMaterial;
                    Debug.Log("SolutionChecker: Applied incorrect material to both symbols (= and /) to create ≠");
                }
            }
            // Способ 2: Прямое управление Renderer'ом
            else if (targetRenderer != null)
            {
                ApplyDirectRendererMaterials(false);
            }
        }

        private void ApplyDirectRendererMaterials(bool isCorrect)
        {
            if (targetRenderer == null) return;

            Material[] materials = targetRenderer.materials;
            
            if (isCorrect)
            {
                // Правильный ответ: показать "=", скрыть "/"
                if (equalsSlotIndex < materials.Length && correctMaterial != null)
                {
                    materials[equalsSlotIndex] = correctMaterial;
                }
                if (slashSlotIndex < materials.Length)
                {
                    materials[slashSlotIndex] = null; // Или прозрачный материал
                }
                Debug.Log("SolutionChecker: Applied correct materials directly to renderer");
            }
            else
            {
                // Неправильный ответ: оба символа "=" и "/" светятся красным для создания "≠"
                if (equalsSlotIndex < materials.Length && incorrectMaterial != null)
                {
                    materials[equalsSlotIndex] = incorrectMaterial;
                }
                if (slashSlotIndex < materials.Length && incorrectMaterial != null)
                {
                    materials[slashSlotIndex] = incorrectMaterial;
                }
                Debug.Log("SolutionChecker: Applied incorrect materials to both symbols (= and /) directly to renderer");
            }
            
            targetRenderer.materials = materials;
        }

        // Публичные методы для настройки из других скриптов
        public void SetTaskLoader(ExampleType1TaskLoader loader)
        {
            taskLoader = loader;
        }

        public void SetAnswer(Trit answerValue)
        {
            answer = answerValue;
        }

        public void ResetErrorCount()
        {
            errorCount = 0;
            Debug.Log("SolutionChecker: Error count reset to 0");
        }

        // Метод для ручной проверки в инспекторе
        [ContextMenu("Test Solution")]
        public void TestSolution()
        {
            CheckSolution();
        }
    }
}