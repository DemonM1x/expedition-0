using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [Header("Scene Management")]
    public string mainMenu = "StartGame";
    public string settingsSceneName = "Settings"; // Сцена настроек
    public string gameSceneName = "Space Greenhouse"; // Сцена игры для загрузки при старте
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Canvas _about;
    
    void Start()
    {
        MainVisible();
    }

    public void StartGame()
    {
        Debug.Log("Pressed startgame");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        Debug.Log("Pressed settings");
        SceneManager.LoadScene(settingsSceneName);
    }

    public void MainMenu(){
        SceneManager.LoadScene(mainMenu);
    }

    public void AboutVisible()
    {
        _canvas.gameObject.SetActive(false);
        _about.gameObject.SetActive(true);
        Debug.Log($"AboutVisible called; " +
                  $"canvas visible: {_canvas.gameObject.activeSelf}, " +
                  $"about visible: {_about.gameObject.activeSelf}");
    }
    public void MainVisible()
    {
        _canvas.gameObject.SetActive(true);
        _about.gameObject.SetActive(false);
        Debug.Log($"MainVisible called; " +
                  $"canvas visible: {_canvas.gameObject.activeSelf}, " +
                  $"about visible: {_about.gameObject.activeSelf}");
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}