using UnityEngine;

public class GameDataHandler : MonoBehaviour
{
    private void Start()
    {
        // Save player data on start
        SavePlayerData();
    }

    public void SavePlayerData()
    {
        int playerGems = PlayerPrefs.GetInt("Gem", 0);
        int playerStars = PlayerPrefs.GetInt("Stars", 0);

        // Save gems and stars to PlayFab
        PlayFabManager.Instance.SavePlayerData("PlayerGems", playerGems.ToString());
        PlayFabManager.Instance.SavePlayerData("PlayerStars", playerStars.ToString());
    }

    public void LoadPlayerData()
    {
        // Retrieve gems and stars from PlayFab
        PlayFabManager.Instance.GetPlayerData("PlayerGems");
        PlayFabManager.Instance.GetPlayerData("PlayerStars");
    }
}
