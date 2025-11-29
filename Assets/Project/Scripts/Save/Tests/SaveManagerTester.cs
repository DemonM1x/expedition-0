// ────────────────────────────────────────
// АВТО-ТЕСТ (работает только в редакторе)
// ────────────────────────────────────────
#if UNITY_EDITOR
using UnityEngine;

namespace Expedition0.Save.Tests
{
    public class SaveManagerTester : MonoBehaviour
    {

        void Start()
        {
            Debug.Log("<color=orange>=== ЗАПУСК АВТО-ТЕСТА SaveManager ===</color>");

            // 0. Получаем что есть

            SaveManager.GetSaveBinary();
            Log("До сброса:");

            // 1. Полная очистка
            SaveManager.ResetSave();
            Log("После ResetSave");

            // 2. Пройдём уровни
            SaveManager.SetCompleted(GameProgress.Level0_Tutorial);
            SaveManager.SetCompleted(GameProgress.Level1_Greenhouse);
            SaveManager.SetCompleted(GameProgress.SeenIllusion);
            Log("После прохождения Tutorial, Greenhouse и Illusion");

            // 3. Проверим отдельные биты
            Debug.Log($"Tutorial пройден? {SaveManager.IsCompleted(GameProgress.Level0_Tutorial)}");
            Debug.Log($"Greenhouse пройден? {SaveManager.IsCompleted(GameProgress.Level1_Greenhouse)}");
            Debug.Log(
                $"MachineHall пройден? {SaveManager.IsCompleted(GameProgress.Level3_MachineHall)} (должен быть false)");
            Debug.Log($"Illusion видена? {SaveManager.IsCompleted(GameProgress.SeenIllusion)}");

            // 4. Пустое ли?
            Debug.Log($"Сохранение пустое? {SaveManager.IsSaveEmpty()} (должно быть false)");

            // 5. Повторный Set — не должен сломать
            SaveManager.SetCompleted(GameProgress.Level1_Greenhouse);
            Log("После повторного SetCompleted (должно быть то же)");

            Debug.Log("<color=green>=== ТЕСТ ЗАВЕРШЁН ===</color>");
            Debug.Log("<color=yellow>Нажми Stop → Play снова → увидишь, что прогресс сохранился!</color>");
        }

        void Log(string step)
        {
            Debug.Log($"<color=cyan>{step}:</color> {SaveManager.GetSaveBinary()}");
        }

        // Опционально: кнопка в инспекторе для сброса
        [ContextMenu("Reset Save (из инспектора)")]
        void ResetFromMenu()
        {
            SaveManager.ResetSave();
            Debug.Log("Сохранение сброшено!");
        }
    }
}
#endif