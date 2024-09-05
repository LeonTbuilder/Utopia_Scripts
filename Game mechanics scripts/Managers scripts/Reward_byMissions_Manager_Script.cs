using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmallHedge.SoundManager;


public class Reward_byMissions_Manager_Script : MonoBehaviour
{
    [System.Serializable]
    public class Mission
    {
        public string missionName;
        [TextArea]
        public string missionDescription;
        public TextMeshProUGUI missionDescriptionText;
        public ObjectiveType objectiveType;
        public Slider missionProgressSlider;
        public TextMeshProUGUI missionValueText;
        public int completeMissionValue;
        public int currentMissionValue;
        public bool rewardClaimed;
        public Button rewardButton;
        public GameObject dimEffect;
        public TextMeshProUGUI rewardText;
        public int rewardAmount;
        public RewardType rewardType;
    }

    public enum RewardType
    {
        Gems,
        SkillPoints
    }

    public enum ObjectiveType
    {
        PinsRemoved,
        EnemiesKilled,
        LevelsPassed,
        AdsWatched,
        EnergyUsed,
        MissionsCompleted,
        BossDefeated,
        SkillsUsed
    }

    public List<Mission> missions;

    public GameObject daily_Panel, achievments_Panel;
    public GameObject daily_Tab, achievments_Tab;
    public GameObject redDot; // Red dot GameObject to indicate claimable reward

    void Start()
    {
        foreach (var mission in missions)
        {
            LoadMissionStatus(mission); // Load mission status from PlayerPrefs
            mission.rewardButton.onClick.AddListener(() => ClaimReward(mission));
            mission.missionDescriptionText.text = mission.missionDescription; // Set the mission description
            UpdateMissionUI(mission); // Initialize the UI
        }

        Open_Daily_Tab();
        UpdateRedDot();
    }

    void Update()
    {
        foreach (var mission in missions)
        {
            UpdateMissionProgress(mission); // Update each mission's progress
        }
        UpdateRedDot();
    }

    public void UpdateMissionProgress(Mission mission)
    {
        // Update current mission value based on the objective type
        mission.currentMissionValue = GetCurrentObjectiveValue(mission.objectiveType);
        mission.missionProgressSlider.value = mission.currentMissionValue;
        mission.missionValueText.text = $"{mission.currentMissionValue}/{mission.completeMissionValue}";
        UpdateMissionUI(mission); // Update the UI
    }

    private int GetCurrentObjectiveValue(ObjectiveType objectiveType)
    {
        // Retrieve the current value from PlayerPrefs based on the objective type
        switch (objectiveType)
        {
            case ObjectiveType.PinsRemoved:
                return PlayerPrefs.GetInt("PinsRemoved", 0);
            case ObjectiveType.EnemiesKilled:
                return PlayerPrefs.GetInt("EnemiesKilled", 0);
            case ObjectiveType.LevelsPassed:
                return PlayerPrefs.GetInt("LevelsPassed", 0);
            case ObjectiveType.AdsWatched:
                return PlayerPrefs.GetInt("AdsWatched", 0);
            case ObjectiveType.EnergyUsed:
                return PlayerPrefs.GetInt("EnergyUsed", 0);
            case ObjectiveType.MissionsCompleted:
                return PlayerPrefs.GetInt("MissionsCompleted", 0);
            case ObjectiveType.BossDefeated:
                return PlayerPrefs.GetInt("BossDefeated", 0);
            case ObjectiveType.SkillsUsed:
                return PlayerPrefs.GetInt("SkillsUsed", 0);
            default:
                return 0;
        }
    }

    private void UpdateMissionUI(Mission mission)
    {
        // Check if the mission is complete and if the reward has been claimed
        if (mission.currentMissionValue >= mission.completeMissionValue && !mission.rewardClaimed)
        {
            mission.rewardButton.interactable = true;
            SetButtonAlpha(mission.rewardButton, 1f);
            mission.dimEffect.SetActive(false);
        }
        else
        {
            mission.rewardButton.interactable = false;
            SetButtonAlpha(mission.rewardButton, 0.3f);
            mission.dimEffect.SetActive(mission.rewardClaimed);
        }

        mission.rewardText.text = $"{mission.rewardAmount} {mission.rewardType.ToString()}";
    }

    private void SetButtonAlpha(Button button, float alpha)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = alpha;
            buttonImage.color = color;
        }
    }

    public void ClaimReward(Mission mission)
    {
        // Grant the reward if the mission is complete and the button is interactable
        if (mission.currentMissionValue >= mission.completeMissionValue && mission.rewardButton.interactable)
        {
            Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Collect_Rewars_Click);

            if (mission.rewardType == RewardType.Gems)
            {
                int currentGems = PlayerPrefs.GetInt("Gem", 0);
                PlayerPrefs.SetInt("Gem", currentGems + mission.rewardAmount);
            }
            else if (mission.rewardType == RewardType.SkillPoints)
            {
                int currentSkillPoints = PlayerPrefs.GetInt("Skill_Points", 0);
                PlayerPrefs.SetInt("Skill_Points", currentSkillPoints + mission.rewardAmount);
            }

            // Activate the dim effect and set the button to not interactable
            mission.dimEffect.SetActive(true);
            mission.rewardButton.interactable = false;
            mission.rewardClaimed = true; // Mark the reward as claimed
            PlayerPrefs.SetInt(mission.missionName + "_rewardClaimed", 1); // Save reward claimed status
            SetButtonAlpha(mission.rewardButton, 0.3f);

            // Increment MissionsCompleted PlayerPrefs
            int missionsCompleted = PlayerPrefs.GetInt("MissionsCompleted", 0);
            PlayerPrefs.SetInt("MissionsCompleted", missionsCompleted + 1);

            UpdateMissionUI(mission); // Update the UI to reflect changes
            UpdateRedDot(); // Update red dot status
        }
    }

    private void SetTabAlpha(GameObject tab, float alpha)
    {
        Image tabImage = tab.GetComponent<Image>();
        if (tabImage != null)
        {
            Color color = tabImage.color;
            color.a = alpha;
            tabImage.color = color;
        }
    }

    public void Open_Achivments_Tab()
    {
        daily_Panel.SetActive(false);
        achievments_Panel.SetActive(true);

        SetTabAlpha(daily_Tab, 0.3f);
        SetTabAlpha(achievments_Tab, 1f);
    }

    public void Open_Daily_Tab()
    {
        daily_Panel.SetActive(true);
        achievments_Panel.SetActive(false);

        SetTabAlpha(daily_Tab, 1f);
        SetTabAlpha(achievments_Tab, 0.3f);
    }

    private void UpdateRedDot()
    {
        bool canClaimReward = false;
        foreach (var mission in missions)
        {
            if (mission.currentMissionValue >= mission.completeMissionValue && !mission.rewardClaimed)
            {
                canClaimReward = true;
                break;
            }
        }
        redDot.SetActive(canClaimReward);
    }

    private void LoadMissionStatus(Mission mission)
    {
        mission.rewardClaimed = PlayerPrefs.GetInt(mission.missionName + "_rewardClaimed", 0) == 1;
    }
}
