using System;
using UnityEngine;
using Expedition0.Util;

namespace Expedition0.Save
{
    [Flags]
    public enum GameProgress
    {
        Level0_Tutorial = 1 << 0, // [0] level 0 (tutorial) → 0b00001 
        Level1_Greenhouse = 1 << 1, // [1] level 1 (greenhouse) → 0b00010
        Level2_OuterSkeleton = 1 << 2, // [2] level 2 (outer skeleton) → 0b00100
        Level3_MachineHall = 1 << 3, // [3] level 3 (machine hall) → 0b01000
        SeenIllusion = 1 << 4, // [4] seen Illusion → 0b10000

        // Комбинации флагов (вспомогательные)
        MainLevels = Level1_Greenhouse | Level2_OuterSkeleton | Level3_MachineHall, // 0b01110
        All = Level0_Tutorial | MainLevels | SeenIllusion //  0b11111
    }

    public static class SaveManager
    {
        private const string SAVE_KEY = "GameProgressBits"; // Ключ для PlayerPrefs
        private const int IllusionThreshold = 2; // Количество пройденных уровней, прежде чем нужно встретить Иллюзию

        /// <summary>
        /// Сохраняет прогресс: отмечает, что пользователь прошел указанный уровень/флаг.
        /// Устанавливает соответствующий бит в 1.
        /// </summary>
        /// <param name="progress">Флаг прогресса</param>
        public static void SetCompleted(GameProgress progress)
        {
            int currentSave = PlayerPrefs.GetInt(SAVE_KEY, 0);
            currentSave |= (int)progress;
            PlayerPrefs.SetInt(SAVE_KEY, currentSave);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Загружает текущее сохранение как целое число (битовую маску).
        /// По умолчанию возвращает 0 (0b00000), если ничего не сохранено.
        /// </summary>
        /// <returns>Битовое число сохранения</returns>
        public static int LoadSave()
        {
            return PlayerPrefs.GetInt(SAVE_KEY, 0);
        }

        /// <summary>
        /// Проверяет, пройден ли конкретный уровень/флаг.
        /// </summary>
        /// <param name="progress">Флаг прогресса</param>
        /// <returns>true, если бит установлен (пройден)</returns>
        public static bool IsCompleted(GameProgress progress)
        {
            int saveData = LoadSave();
            return (saveData & (int)progress) != 0;
        }

        /// <summary>
        /// Проверяет, пустое ли сохранение (ничего не пройдено, == 0b00000).
        /// </summary>
        /// <returns>true, если сохранение пустое</returns>
        public static bool IsSaveEmpty()
        {
            return LoadSave() == 0;
        }

        /// <summary>
        /// Сбрасывает сохранение (для тестирования или новой игры).
        /// </summary>
        public static void ResetSave()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Возвращает прогресс в виде двоичного строкового представления (для отладки).
        /// </summary>
        /// <returns>Строка вроде "0b10100"</returns>
        public static string GetSaveBinary()
        {
            int saveData = LoadSave();
            return "0b" + System.Convert.ToString(saveData, 2).PadLeft(5, '0');
        }

        //Загрузка как enum (удобно для комбинаций)
        public static GameProgress LoadProgress() => (GameProgress)LoadSave();

        /// <summary>
        /// Подсчитывает количество пройденных основных уровней в сохранении.
        /// </summary>
        /// <param name="saveData">Текущее сохранение</param>
        /// <returns>Количество пройденных уровней</returns>
        public static int MainLevelsCompletedCount(int saveData)
        {
            return BitUtils.CountOnes(saveData & (int)GameProgress.MainLevels);
        }

        /// <summary>
        /// Проверяет, не наблюдается ли в сохранении следующих условий:
        /// - установлены какие-то ещё биты, кроме All (может указывать на повреждение сохранения)
        /// - пройдены любые сюжетные уровни, но не пройден туториал;
        /// - пройдены все сюжетные уровни, но не встречена Иллюзия.
        ///
        /// Может быть использовано для реализации простого античита.
        /// </summary>
        /// <param name="saveData">Текущее сохранение</param>
        /// <returns>Является ли сохранение запрещённой комбинацией</returns>
        public static bool IsForbiddenCombination(int saveData)
        {
            bool cond0 = (saveData & ~(int)GameProgress.All) != 0; // Corrupted save
            bool cond1 = (saveData & (int)GameProgress.Level0_Tutorial) == 0 &&
                         (saveData & (int)GameProgress.MainLevels) != 0;
            bool cond2 = (saveData & (int)GameProgress.MainLevels) == (int)GameProgress.MainLevels &&
                         (saveData & (int)GameProgress.SeenIllusion) == 0;
            return cond0 || cond1 || cond2;
        }

        /// <summary>
        /// Проверяет наличие "усложнённого режима" (зависит от числа пройденных уровней).
        /// В данный момент влияет на музыку в лобби и на уровнях.
        /// </summary>
        /// <param name="saveData">Текущее сохранение</param>
        /// <returns>Включён ли усложнённый режим</returns>
        public static bool EncoreModeEnabled(int saveData) =>
            MainLevelsCompletedCount(saveData) >= IllusionThreshold;
    }
}