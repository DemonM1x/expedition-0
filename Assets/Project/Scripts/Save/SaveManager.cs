using UnityEngine;
public enum GameProgress
{
    Level0_Tutorial = 4,    // [0] level 0 (tutorial)
    Level1_Greenhouse = 3,  // [1] level 1 (greenhouse)
    Level2_OuterSkeleton = 2, // [2] level 2 (outer skeleton)
    Level3_MachineHall = 1,   // [3] level 3 (machine hall)
    SeenIllusion = 0         // [4] seen Illusion
}

public static class SaveManager
{
    private const string SAVE_KEY = "GameProgressBits"; // Ключ для PlayerPrefs

    /// <summary>
    /// Сохраняет прогресс: отмечает, что пользователь прошел указанный уровень/флаг.
    /// Устанавливает соответствующий бит в 1.
    /// </summary>
    /// <param name="progress">Флаг прогресса</param>
    public static void SetCompleted(GameProgress progress)
    {
        int currentSave = PlayerPrefs.GetInt(SAVE_KEY, 0);
        currentSave |= (1 << (int)progress); // Устанавливаем бит
        PlayerPrefs.SetInt(SAVE_KEY, currentSave);
        PlayerPrefs.Save(); // Сохраняем немедленно
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
        return (saveData & (1 << (int)progress)) != 0;
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
}