using UnityEngine;
using TMPro;

public class Energy_Text_Script : MonoBehaviour
{
    private TextMeshProUGUI energyText;

    private void Awake()
    {
        energyText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // Ensure the text is updated immediately
        UpdateEnergyText();

        // Subscribe to the event
        if (Energy_Manager_Script._instance != null)
        {
            Energy_Manager_Script._instance.UpdateEnergyUI += UpdateEnergyText;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the event
        if (Energy_Manager_Script._instance != null)
        {
            Energy_Manager_Script._instance.UpdateEnergyUI -= UpdateEnergyText;
        }
    }

    public void UpdateEnergyText()
    {
        if (energyText != null)
        {
            int currentEnergy = PlayerPrefs.GetInt("Energy");
            int maxEnergy = Energy_Manager_Script._instance != null ? Energy_Manager_Script._instance.maxEnergy : 5; // Default value in case _instance is null
            energyText.text = $"{currentEnergy}/{maxEnergy}";
        }
    }
}
