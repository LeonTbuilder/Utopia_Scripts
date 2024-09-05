using UnityEngine;

public class PlayerPref_Manager : MonoBehaviour
{
    void Awake()
    {
        InitializePlayerPrefs();
    }

    void InitializePlayerPrefs()
    {
        if (PlayerPrefs.GetInt("StartGame") == 0)
        {
            PlayerPrefs.SetInt("StartGame", 1);
            PlayerPrefs.SetInt("Gem", 200);
            PlayerPrefs.SetInt("Skill_Points", 0);
            PlayerPrefs.SetInt("Energy", 5);
            PlayerPrefs.SetInt("LevelLocked", 1);
            PlayerPrefs.SetInt("Stars", 0);
            PlayerPrefs.SetInt("PinsRemoved", 0);
            PlayerPrefs.SetInt("EnemiesKilled", 0);
            PlayerPrefs.SetInt("LevelsPassed", 0);
            PlayerPrefs.SetInt("AdsWatched", 0);
            PlayerPrefs.SetInt("EnergyUsed", 0);
            PlayerPrefs.SetInt("MissionsCompleted", 0);
            PlayerPrefs.SetInt("BossDefeated", 0); // Not added yet
            PlayerPrefs.SetInt("SkillsUsed", 0);
        }
    }
}
