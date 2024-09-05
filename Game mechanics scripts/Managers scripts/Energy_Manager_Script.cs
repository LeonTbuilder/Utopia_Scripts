using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Energy_Manager_Script : MonoBehaviour
{
    public static Energy_Manager_Script _instance;

    public int maxEnergy = 5;
    public int currentEnergy;
    public GameObject panel_MoreEnergy;
    public TextMeshProUGUI energyTimerText;

    private DateTime lastEnergyUpdateTime;
    private TimeSpan energyRechargeInterval = TimeSpan.FromMinutes(5);

    public event Action UpdateEnergyUI;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        LoadEnergy();
        CheckAndUpdateEnergy();
        StartCoroutine(EnergyRechargeCoroutine());
    }

    private void OnApplicationQuit()
    {
        SaveLastEnergyUpdateTime();
        PlayerPrefs.Save(); 
    }

    public void LoadEnergy()
    {
        currentEnergy = PlayerPrefs.GetInt("Energy", maxEnergy);
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        lastEnergyUpdateTime = DateTime.Parse(PlayerPrefs.GetString("LastEnergyUpdateTime", DateTime.Now.ToString()));
        UpdateEnergyTextUI();
    }

    public void SaveEnergy()
    {
        PlayerPrefs.SetInt("Energy", currentEnergy);
        SaveLastEnergyUpdateTime();
        PlayerPrefs.Save(); 
        UpdateEnergyTextUI();
    }

    public void SaveLastEnergyUpdateTime()
    {
        PlayerPrefs.SetString("LastEnergyUpdateTime", DateTime.Now.ToString());
        PlayerPrefs.Save(); 
    }

    public void UseEnergy()
    {
        if (currentEnergy > 0)
        {
            currentEnergy--;

            int energyUsed = PlayerPrefs.GetInt("EnergyUsed", 0);
            PlayerPrefs.SetInt("EnergyUsed", energyUsed + 1);

            SaveEnergy(); 
        }
    }

    public void ShowMoreEnergyPanel_ifNo_Energy()
    {
        panel_MoreEnergy.SetActive(true);
    }

    public void Close_EnergyPanel_ifNo_Energy()
    {
        panel_MoreEnergy.SetActive(false);
    }

    public void UpdateEnergyTextUI()
    {
        UpdateEnergyUI?.Invoke();

        UpdateAllSliders();

        if (energyTimerText != null)
        {
            energyTimerText.gameObject.SetActive(currentEnergy < maxEnergy);
        }
    }

    public void RefillEnergy()
    {
        currentEnergy = maxEnergy;
        SaveEnergy(); 
    }

    public void UpdateEnergy(int energy)
    {
        currentEnergy = Mathf.Clamp(energy, 0, maxEnergy);
        SaveEnergy(); 

        UpdateEnergyTextUI();
    }


    public int GetCurrentEnergy()
    {
        return currentEnergy;
    }

    private void UpdateAllSliders()
    {
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
        foreach (Slider slider in sliders)
        {
            if (slider.gameObject.name == "Energy_Slider")
            {
                slider.maxValue = maxEnergy;
                slider.value = currentEnergy;
            }
        }
    }

    public void OnAdWatched()
    {
        RefillEnergy();
        UpdateEnergyTextUI();
    }

    private void CheckAndUpdateEnergy()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSinceLastUpdate = currentTime - lastEnergyUpdateTime;

        int energyToAdd = (int)(timeSinceLastUpdate.TotalMinutes / energyRechargeInterval.TotalMinutes);
        if (energyToAdd > 0)
        {
            currentEnergy = Mathf.Clamp(currentEnergy + energyToAdd, 0, maxEnergy);
            lastEnergyUpdateTime = currentTime;
            SaveEnergy();
        }
    }

    private IEnumerator EnergyRechargeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            DateTime currentTime = DateTime.Now;
            TimeSpan timeSinceLastUpdate = currentTime - lastEnergyUpdateTime;
            TimeSpan timeToNextEnergy = energyRechargeInterval - timeSinceLastUpdate;

            if (timeToNextEnergy <= TimeSpan.Zero)
            {
                currentEnergy = Mathf.Clamp(currentEnergy + 1, 0, maxEnergy);
                lastEnergyUpdateTime = currentTime;
                SaveEnergy();
                timeToNextEnergy = energyRechargeInterval;
            }

            if (energyTimerText != null)
            {
                energyTimerText.text = string.Format("{0:D2}:{1:D2}", timeToNextEnergy.Minutes, timeToNextEnergy.Seconds);
                energyTimerText.gameObject.SetActive(currentEnergy < maxEnergy);
            }

            UpdateEnergyTextUI();
        }
    }
}
