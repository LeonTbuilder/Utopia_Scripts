using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmallHedge.SoundManager;


public class Reward_By_Level_Script : MonoBehaviour
{
    public static Reward_By_Level_Script instance;

    [Header("UI Elements")]
    public Button rewardButton;
    public GameObject redDot;
    public List<Slider> levelProgressSliders;
    public List<TextMeshProUGUI> levelProgressTexts;

    private const int levelsForReward = 10;

    public GameObject rewardPopup;
    public TextMeshProUGUI rewardPopupText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        redDot.SetActive(false);
        UpdateRewardButtonState();
    }

    private void Start()
    {
        UpdateLevelProgress();
        rewardPopup.SetActive(false);
    }

    private void Update()
    {
        int levelsPassed = PlayerPrefs.GetInt("LevelsPassed", 0);
        if (levelsPassed >= levelsForReward)
        {
            redDot.SetActive(true);
            rewardButton.interactable = true;
            SetButtonAlpha(rewardButton, 1f);
        }
        else
        {
            redDot.SetActive(false);
            rewardButton.interactable = false;
            SetButtonAlpha(rewardButton, 0.3f);
        }
        UpdateLevelProgress();
    }

    public void ClaimReward()
    {
        if (rewardButton.interactable)
        {
            Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Collect_Rewars_Click);

            int reward = CalculateReward();
            int currentGems = PlayerPrefs.GetInt("Gem", 0);
            PlayerPrefs.SetInt("Gem", currentGems + reward);

            // Deduct 10 levels instead of resetting to 0
            int levelsPassed = PlayerPrefs.GetInt("LevelsPassed", 0);
            levelsPassed -= levelsForReward;
            PlayerPrefs.SetInt("LevelsPassed", levelsPassed);

            UpdateRewardButtonState();
            UpdateLevelProgress();

            ShowRewardPopup(reward.ToString());
        }
    }

    private int CalculateReward()
    {
        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= 0.05f)
        {
            return Random.Range(801, 1001);
        }
        else if (randomValue <= 0.20f)
        {
            return Random.Range(501, 801);
        }
        else
        {
            return Random.Range(100, 501);
        }
    }

    private void SetButtonAlpha(Button button, float alpha)
    {
        Color color = button.image.color;
        color.a = alpha;
        button.image.color = color;
    }

    private void UpdateRewardButtonState()
    {
        int levelsPassed = PlayerPrefs.GetInt("LevelsPassed", 0);
        if (levelsPassed >= levelsForReward)
        {
            rewardButton.interactable = true;
            SetButtonAlpha(rewardButton, 1f);
        }
        else
        {
            rewardButton.interactable = false;
            SetButtonAlpha(rewardButton, 0.3f);
        }
    }

    private void UpdateLevelProgress()
    {
        int levelsPassed = PlayerPrefs.GetInt("LevelsPassed", 0);
        float progress = (float)levelsPassed / levelsForReward;

        for (int i = 0; i < levelProgressSliders.Count; i++)
        {
            if (levelProgressSliders[i] != null)
            {
                levelProgressSliders[i].value = progress;
            }
        }

        for (int i = 0; i < levelProgressTexts.Count; i++)
        {
            if (levelProgressTexts[i] != null)
            {
                levelProgressTexts[i].text = $"{levelsPassed}/{levelsForReward}";
            }
        }
    }

    private void ShowRewardPopup(string reward)
    {
        rewardPopup.SetActive(true);
        rewardPopupText.text = $"You received {reward} gems";
    }

    public void CloseRewardPopup()
    {
        rewardPopup.SetActive(false);
        Main_Menu_Manager_Script._instance.Close_Panel_Reward_Levels();
    }
}
