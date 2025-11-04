using UnityEngine;
using System.IO;
using TMPro;

public class InsertText : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private string relativePath = "Project/Data/Text/about.txt";
    [TextArea] [SerializeField] private string loadedText;

    void Start()  
    {
        string fullPath = Path.Combine(Application.dataPath, relativePath.Replace("\\", "/"));
        if (File.Exists(fullPath))
        {
            loadedText = File.ReadAllText(fullPath);
            if (targetText == null)
            {
                targetText = GetComponent<TMP_Text>();
            }
            if (targetText != null)
            {
                targetText.text = loadedText;
            }
            else
            {
                Debug.LogWarning("InsertText: TMP_Text компонент не найден на объекте и не назначен в инспекторе.");
            }
        }
        else
        {
            Debug.LogError($"InsertText: Файл не найден: {fullPath}");
        }
    }
}
