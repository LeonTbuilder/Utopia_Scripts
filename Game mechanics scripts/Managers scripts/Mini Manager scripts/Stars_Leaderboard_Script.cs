using UnityEngine;
using TMPro;

public class Stars_Leaderboard_Script : MonoBehaviour
{
    public int currentStars;

    [SerializeField]
    private TextMeshProUGUI starsText;

    private void Start()
    {
        int totalStars = PlayerPrefs.GetInt("Stars");
        UpdateStarsText(totalStars);
    }

    public void UpdateStarsText(int totalStars)
    {
        if (starsText != null)
        {
            starsText.text = totalStars.ToString();
        }
    }
}
