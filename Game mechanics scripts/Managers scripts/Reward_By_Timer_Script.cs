using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmallHedge.SoundManager;


public class Reward_By_Timer_Script : MonoBehaviour
{
    public static Reward_By_Timer_Script instance;

    [Header("UI Elements")]
    public List<TextMeshProUGUI> timerTexts;
    public GameObject rewardPopup;
    public TextMeshProUGUI rewardPopupText;
    public Button rewardButton; 

    private const float rewardInterval = 4 * 60 * 60; // 4 hours in seconds
    private float remainingTime;
    private DateTime lastRewardTime;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        rewardPopup.SetActive(false);
    }

    private void Start()
    {
        LoadRemainingTime();
        StartCoroutine(TimerCoroutine());
    }

    private void LoadRemainingTime()
    {
        string lastRewardTimeString = PlayerPrefs.GetString("LastRewardTime", string.Empty);
        if (!string.IsNullOrEmpty(lastRewardTimeString) && DateTime.TryParse(lastRewardTimeString, out lastRewardTime))
        {
            TimeSpan timeElapsed = DateTime.Now - lastRewardTime;
            remainingTime = Mathf.Max(0, rewardInterval - (float)timeElapsed.TotalSeconds);
        }
        else
        {
            // If there's no saved time or it's invalid, set to full duration
            remainingTime = rewardInterval;
            lastRewardTime = DateTime.Now.AddSeconds(-rewardInterval);
            SaveRemainingTime();
        }
        UpdateRewardButtonState();
        UpdateTimerTexts();
    }

    private void SaveRemainingTime()
    {
        PlayerPrefs.SetString("LastRewardTime", lastRewardTime.ToString());
        PlayerPrefs.Save();
    }

    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    UpdateRewardButtonState();
                }
            }
            UpdateTimerTexts();
            yield return null;
        }
    }

    private void UpdateTimerTexts()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
        string timerText = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";

        foreach (var text in timerTexts)
        {
            if (text != null)
            {
                text.text = timerText;
            }
        }
    }

    public void ClaimReward()
    {
        if (rewardButton.interactable)
        {
            Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Collect_Rewars_Click);

            string reward = CalculateReward();
            lastRewardTime = DateTime.Now;
            remainingTime = rewardInterval;
            SaveRemainingTime();
            UpdateRewardButtonState();
            UpdateTimerTexts();
            ShowRewardPopup(reward);
        }
    }

    private string CalculateReward()
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue <= 0.3f)
        {
            int gems = CalculateGemReward();
            int currentGems = PlayerPrefs.GetInt("Gem", 0);
            PlayerPrefs.SetInt("Gem", currentGems + gems);
            return $"{gems} gems";
        }
        else
        {
            int energy = CalculateEnergyReward();
            int currentEnergy = PlayerPrefs.GetInt("Energy", 0);
            PlayerPrefs.SetInt("Energy", currentEnergy + energy);
            return $"{energy} energy";
        }
    }

    private int CalculateGemReward()
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue <= 0.05f)
        {
            return UnityEngine.Random.Range(801, 1001);
        }
        else if (randomValue <= 0.20f)
        {
            return UnityEngine.Random.Range(501, 801);
        }
        else
        {
            return UnityEngine.Random.Range(100, 501);
        }
    }

    private int CalculateEnergyReward()
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue <= 0.05f)
        {
            return 3;
        }
        else if (randomValue <= 0.15f)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    private void ShowRewardPopup(string reward)
    {
        rewardPopup.SetActive(true);
        rewardPopupText.text = $"You received {reward}";
    }

    public void CloseRewardPopup()
    {
        rewardPopup.SetActive(false);
        Main_Menu_Manager_Script._instance.Close_Panel_Reward_Timer();
    }

    private void SetButtonAlpha(Button button, float alpha)
    {
        Color color = button.image.color;
        color.a = alpha;
        button.image.color = color;
    }

    private void UpdateRewardButtonState()
    {
        if (remainingTime > 0)
        {
            rewardButton.interactable = false;
            SetButtonAlpha(rewardButton, 0.3f);
        }
        else
        {
            rewardButton.interactable = true;
            SetButtonAlpha(rewardButton, 1f);
        }
    }
}
