using UnityEngine;
public enum GameProgress
{
    Level0_Tutorial = 1 << 4, // [0] level 0 (tutorial) → 0b10000 
    Level1_Greenhouse = 1 << 3, // [1] level 1 (greenhouse) → 0b01000
    Level2_OuterSkeleton = 1 << 2, // [2] level 2 (outer skeleton) → 0b00100
    Level3_MachineHall = 1 << 1, // [3] level 3 (machine hall) → 0b00010
    SeenIllusion = 1 << 0  // [4] seen Illusion → 0b00001 
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
}