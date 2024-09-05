using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SmallHedge.SoundManager;

public class Main_Menu_Manager_Script : MonoBehaviour
{
    public static Main_Menu_Manager_Script _instance;

    [Header("Drag Panel")]
    public GameObject panel_Level_Select, panel_Lobby, panel_Loading, panel_Settings, panel_Add_Energy, panel_Full_Energy,
                      panel_Add_Gems, panel_Terms, panel_Missions, panel_Reward_byLevel, panel_Reward_byTimer;

    [Header("Drag Images")]
    public Image img_Loading_MaskVeil;

    [Header("Drag Text")]
    public List<TextMeshProUGUI> text_Gems;
    public TextMeshProUGUI text_Current_Lvl;

    [Header("Level Progress")]
    public Slider levelProgressSlider;
    public TextMeshProUGUI levelProgressText;
    private const int maxLevel = 1000;

    [Header("Background Mask")]
    public GameObject BG_Mask;

    private int lastGemCount = -1;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        CheckTermsAcceptance();
    }

    private void Start()
    {
        Update_Text_Gems();
        Update_Locked_Level();
        Update_Settings();
        Update_Level_Progress();

        // Stop any gameplay music (e.g., win/lose music) when the main menu is loaded
        Sound_Manager_Script.StopAllSounds();
        Sound_Manager_Script.Play_Menu_Sound(Menu_SoundType.Soundtrack);

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "Main_Menu")
        {
            Sound_Manager_Script.StopAllSounds();
        }
    }

    public void ShowLobbyPanel()
    {
        panel_Lobby.SetActive(true);
        Update_Text_Gems();
        Update_Locked_Level();
        Hero_Menu_Position_Script._instance.Instantiated_Hero();
    }

    private void Update()
    {
        int currentGemCount = PlayerPrefs.GetInt("Gem");
        if (currentGemCount != lastGemCount)
        {
            Update_Text_Gems();
            lastGemCount = currentGemCount;
        }
    }

    public void Update_Text_Gems()
    {
        int gemCount = PlayerPrefs.GetInt("Gem");
        foreach (var text in text_Gems)
        {
            if (text != null)
                text.text = gemCount.ToString();
        }
    }

    void Update_Locked_Level()
    {
        if (text_Current_Lvl != null)
            text_Current_Lvl.text = PlayerPrefs.GetInt("CurrentLevel").ToString();
    }

    public void Update_Settings()
    {
        // Load and apply settings like sound volume, etc. from PlayerPrefs
    }

    public void Load_Level_inSelector(int _lvl)
    {
        PlayerPrefs.SetInt("CurrentLevel", _lvl);

        Close_Panel_Level_Selector();

        StartCoroutine(FadeSceneLoad("Main_Game", 0.35f));
        Close_Panel_Logic();
    }

    IEnumerator FadeSceneLoad(string sceneName, float duration)
    {
        float alpha = 1.0f;
        panel_Loading.SetActive(true);
        SceneManager.LoadScene(sceneName);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / duration;
            img_Loading_MaskVeil.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        panel_Loading.SetActive(false);
    }

    public void Update_Energy(int _energy)
    {
        Energy_Manager_Script._instance.UpdateEnergy(_energy);
    }

    public void Open_Panel_Level()
    {
        panel_Level_Select.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Panel_Level_Selector()
    {
        panel_Level_Select.SetActive(false);
        Close_Panel_Logic();
    }

    public void Open_Panel_Settings()
    {
        panel_Settings.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Panel_Settings()
    {
        panel_Settings.SetActive(false);
        Close_Panel_Logic();
    }

    public void Open_Panel_Energy()
    {
        panel_Add_Energy.SetActive(true);
        Open_Panel_Logic();

        Update_Energy(PlayerPrefs.GetInt("Energy"));
    }

    public void Close_Panel_Energy()
    {
        panel_Add_Energy.SetActive(false);
        Close_Panel_Logic();
    }

    public void Open_Panel_Gems()
    {
        panel_Add_Gems.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Panel_More_Gems()
    {
        panel_Add_Gems.SetActive(false);
        Close_Panel_Logic();
    }

    public void Show_Gem_Ad()
    {
        Reward_Per_Ad.Instance.Show_Gem_Ad();
    }

    public void Show_Energy_Ad()
    {
        if (Energy_Manager_Script._instance.currentEnergy == Energy_Manager_Script._instance.maxEnergy)
        {
            panel_Full_Energy.SetActive(true);
        }
        else
        {
            Reward_Per_Ad.Instance.Show_Energy_Ad();
        }
    }

    public void Close_Panel_fullEnergy()
    {
        panel_Full_Energy.SetActive(false);
    }

    public void Open_Panel_Missions()
    {
        panel_Missions.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Panel_Missions()
    {
        panel_Missions.SetActive(false);
        Close_Panel_Logic();
    }

    public void Open_Terms_OS_Panel()
    {
        panel_Terms.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Terms_OS_Panel()
    {
        panel_Terms.SetActive(false);
        Close_Panel_Logic();
    }

    public void Open_Panel_Reward_Levels()
    {
        panel_Reward_byLevel.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Panel_Reward_Levels()
    {
        panel_Reward_byLevel.SetActive(false);
        Close_Panel_Logic();
    }

    public void Open_Panel_Reward_Timer()
    {
        panel_Reward_byTimer.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Panel_Reward_Timer()
    {
        panel_Reward_byTimer.SetActive(false);
        Close_Panel_Logic();
    }

    void Open_Panel_Logic()
    {
        BG_Mask.SetActive(true);
        Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Open_Panel_Click);
    }

    void Close_Panel_Logic()
    {
        BG_Mask.SetActive(false);
        Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Close_Panel_Click);
    }

    public void Update_Current_Level()
    {
        if (text_Current_Lvl != null)
            text_Current_Lvl.text = PlayerPrefs.GetInt("CurrentLevel").ToString();
    }

    public void Update_Level_Progress()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        levelProgressSlider.maxValue = maxLevel;
        levelProgressSlider.value = currentLevel;

        levelProgressText.text = $"{currentLevel} / {maxLevel}";
    }

    private void CheckTermsAcceptance()
    {
        if (PlayerPrefs.GetInt("TermsAccepted", 0) == 0)
        {
            panel_Terms.SetActive(true);
            BG_Mask.SetActive(true);
        }
        else
        {
            panel_Lobby.SetActive(true);
        }
    }
}
