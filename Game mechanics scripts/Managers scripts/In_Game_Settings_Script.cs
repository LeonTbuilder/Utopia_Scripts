using UnityEngine;
using UnityEngine.UI;
using SmallHedge.SoundManager;

public class In_Game_Settings_Script : MonoBehaviour
{
    [Header("Sound Components")]
    public Slider sfxSlider;
    public Slider musicSlider;

    [Header("Screen Shake")]
    public GameObject screenShakeOnObject;
    public GameObject screenShakeOffObject;

    [Header("Vibration")]
    public GameObject vibrationOnObject;
    public GameObject vibrationOffObject;

    // PlayerPrefs keys for saving settings
    private const string SfxVolumeKey = "SfxVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string ScreenShakeKey = "ScreenShake";
    private const string VibrationKey = "Vibration";

    private bool screenShakeEnabled = true;
    private bool vibrationEnabled = true;

    void Start()
    {
        LoadSettings();
        ApplyAudioSettings();
        ApplyToggleSettings();
    }

    // Load settings from PlayerPrefs
    void LoadSettings()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        screenShakeEnabled = PlayerPrefs.GetInt(ScreenShakeKey, 1) == 1;
        vibrationEnabled = PlayerPrefs.GetInt(VibrationKey, 1) == 1;
    }

    // Save current settings to PlayerPrefs
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxSlider.value);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicSlider.value);
        PlayerPrefs.SetInt(ScreenShakeKey, screenShakeEnabled ? 1 : 0);
        PlayerPrefs.SetInt(VibrationKey, vibrationEnabled ? 1 : 0);

        PlayerPrefs.Save();
    }

    // Called when SFX slider value changes
    public void OnSfxVolumeChanged(float value)
    {
        ApplyAudioSettings();
        SaveSettings();
    }

    // Called when Music slider value changes
    public void OnMusicVolumeChanged(float value)
    {
        ApplyAudioSettings();
        SaveSettings();
    }

    void ApplyAudioSettings()
    {
        Sound_Manager_Script.SetSfxVolume(sfxSlider.value);
        Sound_Manager_Script.SetMusicVolume(musicSlider.value);
    }

    void Apply_ScreenShake_Settings()
    {
        screenShakeOnObject.SetActive(screenShakeEnabled);
        screenShakeOffObject.SetActive(!screenShakeEnabled);
    }

    public void ToggleScreenShake()
    {
        screenShakeEnabled = !screenShakeEnabled;
        Apply_ScreenShake_Settings();
        SaveSettings();
    }

    void Apply_Vibration_Settings()
    {
        vibrationOnObject.SetActive(vibrationEnabled);
        vibrationOffObject.SetActive(!vibrationEnabled);
    }

    public void ToggleVibration()
    {
        vibrationEnabled = !vibrationEnabled;
        Apply_Vibration_Settings();
        SaveSettings();
    }

    void ApplyToggleSettings()
    {
        Apply_ScreenShake_Settings();
        Apply_Vibration_Settings();
    }

    void OnApplicationQuit()
    {
        SaveSettings();
    }
}
