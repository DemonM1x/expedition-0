using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro; 
public class AudioSettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    
    [Header("Volume Parameters")]
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "MusicVolume";
    public string voiceVolumeParam = "VoiceVolume";
    public string sfxVolumeParam = "SFXVolume";
    
    [Header("Volume Step")]
    [Range(5, 20)]
    public float volumeStep = 15f; 
    
    [Header("UI Elements - Buttons and TextMeshPro")]
    public Button masterPlusBtn;
    public Button masterMinusBtn;
    public TMP_Text masterVolumeText; 
    
    public Button musicPlusBtn;
    public Button musicMinusBtn;
    public TMP_Text musicVolumeText; 
    public Button voicePlusBtn;
    public Button voiceMinusBtn;
    public TMP_Text voiceVolumeText; 
    public Button sfxPlusBtn;
    public Button sfxMinusBtn;
    public TMP_Text sfxVolumeText; 
    [Header("Current Volume Values")]
    [Range(0, 100)]
    private float masterVolume = 80f;
    [Range(0, 100)]
    private float musicVolume = 70f;
    [Range(0, 100)]
    private float voiceVolume = 90f;
    [Range(0, 100)]
    private float sfxVolume = 80f;

    private void Start()
    {
        FindTextMeshProElements();
        LoadVolumeSettings();
        SetupButtonListeners();
        ApplyAllVolumes();
        UpdateAllVolumeTexts();
    }

    private void FindTextMeshProElements()
    {
        if (masterVolumeText == null)
            masterVolumeText = GameObject.Find("MasterVolumeText")?.GetComponent<TMP_Text>();
        
        if (musicVolumeText == null)
            musicVolumeText = GameObject.Find("MusicVolumeText")?.GetComponent<TMP_Text>();
            
        if (voiceVolumeText == null)
            voiceVolumeText = GameObject.Find("VoiceVolumeText")?.GetComponent<TMP_Text>();
            
        if (sfxVolumeText == null)
            sfxVolumeText = GameObject.Find("SFXVolumeText")?.GetComponent<TMP_Text>();

        if (masterVolumeText == null || musicVolumeText == null || voiceVolumeText == null || sfxVolumeText == null)
        {
            TMP_Text[] allTexts = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
            foreach (TMP_Text text in allTexts)
            {
                if (text.name.Contains("Master") && masterVolumeText == null)
                    masterVolumeText = text;
                else if (text.name.Contains("Music") && musicVolumeText == null)
                    musicVolumeText = text;
                else if (text.name.Contains("Voice") && voiceVolumeText == null)
                    voiceVolumeText = text;
                else if (text.name.Contains("SFX") && sfxVolumeText == null)
                    sfxVolumeText = text;
            }
        }
    }

    private void SetupButtonListeners()
    {
        if (masterPlusBtn != null)
            masterPlusBtn.onClick.AddListener(() => ChangeMasterVolume(volumeStep));
        
        if (masterMinusBtn != null)
            masterMinusBtn.onClick.AddListener(() => ChangeMasterVolume(-volumeStep));

        if (musicPlusBtn != null)
            musicPlusBtn.onClick.AddListener(() => ChangeMusicVolume(volumeStep));
        
        if (musicMinusBtn != null)
            musicMinusBtn.onClick.AddListener(() => ChangeMusicVolume(-volumeStep));

        if (voicePlusBtn != null)
            voicePlusBtn.onClick.AddListener(() => ChangeVoiceVolume(volumeStep));
        
        if (voiceMinusBtn != null)
            voiceMinusBtn.onClick.AddListener(() => ChangeVoiceVolume(-volumeStep));

        if (sfxPlusBtn != null)
            sfxPlusBtn.onClick.AddListener(() => ChangeSFXVolume(volumeStep));
        
        if (sfxMinusBtn != null)
            sfxMinusBtn.onClick.AddListener(() => ChangeSFXVolume(-volumeStep));
    }

    public void ChangeMasterVolume(float changeAmount)
    {
        masterVolume = Mathf.Clamp(masterVolume + changeAmount, 0f, 100f);
        SetMasterVolume(masterVolume / 100f);
        SaveVolumeSetting(masterVolumeParam, masterVolume / 100f);
        UpdateVolumeText(masterVolumeText, masterVolume);
        Debug.Log($"Master Volume: {masterVolume}%");
    }

    public void ChangeMusicVolume(float changeAmount)
    {
        musicVolume = Mathf.Clamp(musicVolume + changeAmount, 0f, 100f);
        SetMusicVolume(musicVolume / 100f);
        SaveVolumeSetting(musicVolumeParam, musicVolume / 100f);
        UpdateVolumeText(musicVolumeText, musicVolume);
        Debug.Log($"Music Volume: {musicVolume}%");
    }

    public void ChangeVoiceVolume(float changeAmount)
    {
        voiceVolume = Mathf.Clamp(voiceVolume + changeAmount, 0f, 100f);
        SetVoiceVolume(voiceVolume / 100f);
        SaveVolumeSetting(voiceVolumeParam, voiceVolume / 100f);
        UpdateVolumeText(voiceVolumeText, voiceVolume);
        Debug.Log($"Voice Volume: {voiceVolume}%");
    }

    public void ChangeSFXVolume(float changeAmount)
    {
        sfxVolume = Mathf.Clamp(sfxVolume + changeAmount, 0f, 100f);
        SetSFXVolume(sfxVolume / 100f);
        SaveVolumeSetting(sfxVolumeParam, sfxVolume / 100f);
        UpdateVolumeText(sfxVolumeText, sfxVolume);
        Debug.Log($"SFX Volume: {sfxVolume}%");
    }

    public void SetMasterVolume(float volume)
    {
        SetVolume(masterVolumeParam, volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetVolume(musicVolumeParam, volume);
    }

    public void SetVoiceVolume(float volume)
    {
        SetVolume(voiceVolumeParam, volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetVolume(sfxVolumeParam, volume);
    }

    private void SetVolume(string parameter, float volume)
    {
        if (audioMixer != null)
        {
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat(parameter, dB);
        }
    }

    private void UpdateVolumeText(TMP_Text volumeText, float volumePercent)
    {
        if (volumeText != null)
        {
            volumeText.text = $"{Mathf.RoundToInt(volumePercent)}%";
        }
    }

    private void UpdateAllVolumeTexts()
    {
        UpdateVolumeText(masterVolumeText, masterVolume);
        UpdateVolumeText(musicVolumeText, musicVolume);
        UpdateVolumeText(voiceVolumeText, voiceVolume);
        UpdateVolumeText(sfxVolumeText, sfxVolume);
    }

    private void SaveVolumeSetting(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(masterVolumeParam, 0.8f) * 100f;
        musicVolume = PlayerPrefs.GetFloat(musicVolumeParam, 0.7f) * 100f;
        voiceVolume = PlayerPrefs.GetFloat(voiceVolumeParam, 0.9f) * 100f;
        sfxVolume = PlayerPrefs.GetFloat(sfxVolumeParam, 0.8f) * 100f;
    }

    private void ApplyAllVolumes()
    {
        SetMasterVolume(masterVolume / 100f);
        SetMusicVolume(musicVolume / 100f);
        SetVoiceVolume(voiceVolume / 100f);
        SetSFXVolume(sfxVolume / 100f);
    }

    public void ResetToDefaults()
    {
        masterVolume = 80f;
        musicVolume = 70f;
        voiceVolume = 90f;
        sfxVolume = 80f;

        ApplyAllVolumes();
        UpdateAllVolumeTexts();

        SaveVolumeSetting(masterVolumeParam, masterVolume / 100f);
        SaveVolumeSetting(musicVolumeParam, musicVolume / 100f);
        SaveVolumeSetting(voiceVolumeParam, voiceVolume / 100f);
        SaveVolumeSetting(sfxVolumeParam, sfxVolume / 100f);

        Debug.Log("Audio settings reset to defaults");
    }

    [ContextMenu("Check TextMeshPro References")]
    public void CheckTextReferences()
    {
        Debug.Log($"Master TMP_Text: {masterVolumeText != null}");
        Debug.Log($"Music TMP_Text: {musicVolumeText != null}");
        Debug.Log($"Voice TMP_Text: {voiceVolumeText != null}");
        Debug.Log($"SFX TMP_Text: {sfxVolumeText != null}");
    }

    [ContextMenu("Print Current Volumes")]
    public void PrintCurrentVolumes()
    {
        Debug.Log($"Master: {masterVolume}%");
        Debug.Log($"Music: {musicVolume}%");
        Debug.Log($"Voice: {voiceVolume}%");
        Debug.Log($"SFX: {sfxVolume}%");
    }
}