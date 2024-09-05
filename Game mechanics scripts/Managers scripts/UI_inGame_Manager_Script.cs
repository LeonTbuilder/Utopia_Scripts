using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using SmallHedge.SoundManager;

public class UI_inGame_Manager_Script : MonoBehaviour
{
    public static UI_inGame_Manager_Script _instance;

    [Header("Drag Objects")]
    public GameObject panel_Game_UI, panel_Loading, panel_Win_result, panel_Fail_Result, settings_Panel, redo_Panel, hint_Panel, panel_Full_Energy;
    public GameObject BG_Mask;

    [Header("Drag Texts")]
    public List<TextMeshProUGUI> levelTexts = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> gemsTexts = new List<TextMeshProUGUI>();
    TextMeshProUGUI text_Gems_inBonus;

    private SpriteRenderer loadingSpriteRenderer;

    private Vector3 initialCameraPosition;
    private float initialOrthographicSize;

    private void Awake()
    {
        _instance = this;

        loadingSpriteRenderer = panel_Loading.GetComponent<SpriteRenderer>();

        initialCameraPosition = Camera.main.transform.localPosition;
        initialOrthographicSize = Camera.main.orthographicSize;

        ResetAllPanels();
        StartCoroutine(Level_Load_Fading_Screen());

    }

    private void Update()
    {
        UpdateGemsTexts("+ " + Game_Manager_Script.instance.gems_Bonus);
    }

    private void ResetAllPanels()
    {
        panel_Loading.SetActive(false);
        panel_Win_result.SetActive(false);
        panel_Fail_Result.SetActive(false);
        settings_Panel.SetActive(false);
        redo_Panel.SetActive(false);
        Close_Panel_Logic();
        ResetCameraPosition();
    }

    private void ResetCameraPosition()
    {
        Camera.main.transform.localPosition = initialCameraPosition;
        Camera.main.orthographicSize = initialOrthographicSize;
    }

    public void Open_on_Click()
    {
        Application.OpenURL(""); // Add URL
    }

    public void Level_Load(int _level)
    {
        Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Level_Click);

        if ((PlayerPrefs.GetInt("Energy") > 0))
        {
            StartCoroutine(Level_Load_Fading_Screen());
            Energy_Manager_Script._instance.UseEnergy();
        }
        else if ((PlayerPrefs.GetInt("Energy") <= 0))
        {
            Energy_Manager_Script._instance.ShowMoreEnergyPanel_ifNo_Energy();
        }
    }

    private IEnumerator Level_Load_Fading_Screen()
    {
        ResetAllPanels();
        Levels_Manager_Script._instance.CleanLevel();
        Game_Manager_Script.instance.Game_is_Reset();

        panel_Loading.SetActive(true);

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (currentLevel == 0)
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", 1);
        }

        UpdateLevelTexts("LEVEL " + currentLevel);

        if (loadingSpriteRenderer != null)
        {
            yield return StartCoroutine(Effect_Fade_Image(true, loadingSpriteRenderer));
        }

        Levels_Manager_Script._instance.LoadLevel();

        panel_Loading.SetActive(false);
        panel_Game_UI.SetActive(true);
        BG_Mask.SetActive(false);
    }

    private IEnumerator Effect_Fade_Image(bool fadeAway, SpriteRenderer img)
    {
        if (fadeAway)
        {
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        else
        {
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }

    public void Back_toHomescene_From_GamePlay()
    {
        Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Level_Click);

        StartCoroutine(LoadMainMenuAndShowLobby());
        Close_Panel_Logic();
    }

    private IEnumerator LoadMainMenuAndShowLobby()
    {
        yield return SceneManager.LoadSceneAsync("Main_Menu");
        Main_Menu_Manager_Script._instance.ShowLobbyPanel();
    }

    public void Replay_Game()
    {
        Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Level_Click);

        if ((PlayerPrefs.GetInt("Energy") > 0))
        {
            redo_Panel.SetActive(false);
            Close_Panel_Logic();

            Energy_Manager_Script._instance.UseEnergy();

            StartCoroutine(Replay_Game_Coroutine());
        }
        else if ((PlayerPrefs.GetInt("Energy") <= 0))
        {
            Energy_Manager_Script._instance.ShowMoreEnergyPanel_ifNo_Energy();
        }
    }

    private IEnumerator Replay_Game_Coroutine()
    {
        yield return StartCoroutine(Effect_Fade_Image(true, loadingSpriteRenderer));

        Levels_Manager_Script._instance.CleanLevel();
        Game_Manager_Script.instance.Game_is_Reset();

        ResetAllPanels();

        yield return new WaitForSeconds(0.1f);

        Levels_Manager_Script._instance.LoadLevel();
    }

    public void Show_Game_Clear()
    {
        var levelScript = Levels_Manager_Script._instance.GetCurrentLevelType();

        switch (levelScript)
        {
            case Level_Script.Type_ofLEVEL.ENEMY:
                Game_Manager_Script.instance.gems_Bonus += 30;
                break;
            case Level_Script.Type_ofLEVEL.CHEST:
                Game_Manager_Script.instance.gems_Bonus += 100;
                break;
            case Level_Script.Type_ofLEVEL.BUDDY:
                Game_Manager_Script.instance.gems_Bonus += 50;
                break;
            case Level_Script.Type_ofLEVEL.GEMS:
                Game_Manager_Script.instance.gems_Bonus += 60;
                break;
        }

        UpdateGemsTexts("+ " + Game_Manager_Script.instance.gems_Bonus);
        PlayerPrefs.SetInt("Energy", Game_Manager_Script.instance.total_Energy);

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        int lockLevel = PlayerPrefs.GetInt("LevelLocked");

        if (currentLevel >= lockLevel)
        {
            PlayerPrefs.SetInt("LevelLocked", currentLevel + 1);
        }

        UpdateLevelTexts("LEVEL " + currentLevel.ToString());

        StartCoroutine(CenterCameraOnHero(true));  // Passing true for game win

    }

    public void Show_Game_Over()
    {
        Update_Energy(Game_Manager_Script.instance.total_Energy);

        PlayerPrefs.SetInt("Gem", Game_Manager_Script.instance.gems_Current);
        PlayerPrefs.SetInt("Energy", Game_Manager_Script.instance.total_Energy);

        StartCoroutine(CenterCameraOnHero(false)); // Passing false for game over

    }

    private IEnumerator CenterCameraOnHero(bool gameWon)
    {
        yield return new WaitForSeconds(1f);

        if (gameWon)
        {
            panel_Win_result.SetActive(true);
        }
        else
        {
            panel_Fail_Result.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);


        var hero = GameObject.FindGameObjectWithTag("Hero");

        if (hero != null)
        {
            Vector3 startPosition = Camera.main.transform.localPosition;

            
            Vector3 targetPosition = new Vector3(
                startPosition.x,  
                hero.transform.localPosition.y - 0.3f,  
                startPosition.z);  

            float startOrthographicSize = Camera.main.orthographicSize;
            float targetOrthographicSize = 5.0f;

            float duration = 1.0f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                Camera.main.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                Camera.main.orthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Camera.main.transform.localPosition = targetPosition;
            Camera.main.orthographicSize = targetOrthographicSize;
        }
    }

    public void NextLevel()
    {
        Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Level_Click);

        if ((PlayerPrefs.GetInt("Energy") > 0))
        {
            panel_Win_result.SetActive(false);
            Energy_Manager_Script._instance.UseEnergy();
            StartCoroutine(LoadNextLevel_Coroutine());
        }
        else if ((PlayerPrefs.GetInt("Energy") <= 0))
        {
            Energy_Manager_Script._instance.ShowMoreEnergyPanel_ifNo_Energy();
        }
    }

    private IEnumerator LoadNextLevel_Coroutine()
    {
        ResetAllPanels();
        panel_Loading.SetActive(true);

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);

        int lockLevel = PlayerPrefs.GetInt("LevelLocked");
        if (currentLevel >= lockLevel)
        {
            PlayerPrefs.SetInt("LevelLocked", currentLevel + 1);
        }

        PlayerPrefs.SetInt("Gem", Game_Manager_Script.instance.gems_Current + Game_Manager_Script.instance.gems_Bonus);

        yield return SceneManager.LoadSceneAsync("Main_Game");

        StartCoroutine(Level_Load_Fading_Screen());
    }

    public void Get_More_Energy()
    {
        Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Level_Click);

        if ( Game_Manager_Script.instance.total_Energy < 3)
        {
            Game_Manager_Script.instance.total_Energy++;

            PlayerPrefs.SetInt("Gem", Game_Manager_Script.instance.gems_Current);
            PlayerPrefs.SetInt("Energy", Game_Manager_Script.instance.total_Energy);

            Update_Energy(Game_Manager_Script.instance.total_Energy);

            panel_Game_UI.transform.Find("B_Replay").gameObject.SetActive(true);
            panel_Game_UI.transform.Find("B_Skip").gameObject.SetActive(true);
        }
    }

    public void Update_Energy(int _energy)
    {
        Energy_Manager_Script._instance.UpdateEnergy(_energy);
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


    public void UpdateLevelTexts(string text)
    {
        foreach (TextMeshProUGUI levelText in levelTexts)
        {
            levelText.text = text;
        }
    }

    public void UpdateGemsTexts(string text)
    {
        foreach (TextMeshProUGUI gemsText in gemsTexts)
        {
            gemsText.text = text;
        }
    }

    public void ShowGemsText(int gems)
    {
        UpdateGemsTexts(gems.ToString());
    }

    public void Open_Settings_Panel()
    {
        settings_Panel.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Settings_Panel()
    {
        settings_Panel.SetActive(false);
        Close_Panel_Logic();
    }

    public void Open_Hint_Panel()
    {
        hint_Panel.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Hint_Panel()
    {
        hint_Panel.SetActive(false);
        Close_Panel_Logic();
    }

    public void Open_Redo_Panel()
    {
        redo_Panel.SetActive(true);
        Open_Panel_Logic();
    }

    public void Close_Redo_Panel()
    {
        redo_Panel.SetActive(false);
        Close_Panel_Logic();
    }

    void Open_Panel_Logic()
    {
        BG_Mask.SetActive(true);
        Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Open_Panel_Click);
        Time.timeScale = 0;
    }

    void Close_Panel_Logic()
    {
        BG_Mask.SetActive(false);
        Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Close_Panel_Click);
        Time.timeScale = 1;
    }

}
