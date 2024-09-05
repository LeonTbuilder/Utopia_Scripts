using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SmallHedge.SoundManager;

public class Level_Selector_Script : MonoBehaviour
{
    [SerializeField]
    Transform[] list_Level_Objects = new Transform[20]; // Can hold up to 20 level buttons in layout

    [HideInInspector]
    public int selector_Curent_Page = 0;
    private int selector_Total_Page;

    [SerializeField]
    private TextMeshProUGUI text_page;
    [SerializeField]
    private Button button_Previous_Page;

    [SerializeField]
    private Sprite img_Lvlbtn_Unlocked;
    [SerializeField]
    private Sprite star_On;
    [SerializeField]
    private Sprite star_Off;

    private void Awake()
    {
        selector_Curent_Page = PlayerPrefs.GetInt("LevelLocked", 1) / 20;
    }

    private void Start()
    {
        Transform holdersGrid = GetChildByName(transform, "Holders_Grid_Level");

        if (holdersGrid == null) return;

        for (int i = 0; i < holdersGrid.childCount && i < list_Level_Objects.Length; i++)
        {
            list_Level_Objects[i] = holdersGrid.GetChild(i);
        }

        selector_Total_Page = Mathf.CeilToInt((float)holdersGrid.childCount / list_Level_Objects.Length);
        ShowLevelItemInfo();
    }

    private Transform GetChildByName(Transform parent, string name)
    {
        foreach (Transform t in parent.GetComponentsInChildren<Transform>())
        {
            if (t.name == name)
            {
                return t;
            }
        }
        return null;
    }

    private void ShowLevelItemInfo()
    {
        text_page.text = $"PAGE {selector_Curent_Page + 1}";
        int lockedLevel = PlayerPrefs.GetInt("LevelLocked", 1);

        for (int i = 0; i < list_Level_Objects.Length; i++)
        {
            int levelNumber = i + 1 + list_Level_Objects.Length * selector_Curent_Page;

            Transform levelButtonTransform = list_Level_Objects[i].Find("Level_Btn");
            Transform starsHolderTransform = levelButtonTransform.Find("Stars_Holder");

            TextMeshProUGUI levelButtonText = levelButtonTransform.Find("Level_Btn_Text")?.GetComponent<TextMeshProUGUI>();
            Button levelButton = levelButtonTransform.GetComponent<Button>();
            Image buttonImage = levelButtonTransform.GetComponent<Image>();

            levelButtonText.text = levelNumber.ToString();

            if (levelNumber <= lockedLevel)
            {
                buttonImage.sprite = img_Lvlbtn_Unlocked;
                levelButton.interactable = true;
                starsHolderTransform.gameObject.SetActive(true);

                int capturedLevelNumber = levelNumber;
                levelButton.onClick.RemoveAllListeners();
                levelButton.onClick.AddListener(() => LoadLevel(capturedLevelNumber));
            }
            else
            {
                levelButton.interactable = false;
                starsHolderTransform.gameObject.SetActive(false);
            }

            UpdateStars(levelButtonTransform, levelNumber);
        }

        button_Previous_Page.gameObject.SetActive(selector_Curent_Page > 0);
    }

    private void UpdateStars(Transform levelButtonTransform, int levelNumber)
    {
        Transform starsHolder = levelButtonTransform.Find("Stars_Holder");
        if (starsHolder == null) return;

        string leftStarKey = $"Level{levelNumber}_LeftStar";
        string middleStarKey = $"Level{levelNumber}_MiddleStar";
        string rightStarKey = $"Level{levelNumber}_RightStar";

        bool leftStar = PlayerPrefs.GetInt(leftStarKey, 0) == 1;
        bool middleStar = PlayerPrefs.GetInt(middleStarKey, 0) == 1;
        bool rightStar = PlayerPrefs.GetInt(rightStarKey, 0) == 1;

        SetStarImage(starsHolder.Find("Star_Left"), leftStar);
        SetStarImage(starsHolder.Find("Star_Middle"), middleStar);
        SetStarImage(starsHolder.Find("Star_Right"), rightStar);
    }

    private void SetStarImage(Transform starTransform, bool isActive)
    {
        if (starTransform == null) return;
        Image starImage = starTransform.GetComponent<Image>();
        starImage.sprite = isActive ? star_On : star_Off;
        starImage.enabled = true; 
    }

    public void LoadLevel(int level)
    {
        if (level <= PlayerPrefs.GetInt("LevelLocked") && (PlayerPrefs.GetInt("Energy") > 0))
        {
            Sound_Manager_Script.Play_UI_Sound(UI_SoundType.Level_Click);

            Main_Menu_Manager_Script._instance.Load_Level_inSelector(level);
            Main_Menu_Manager_Script._instance.panel_Lobby.SetActive(false);
            Energy_Manager_Script._instance.UseEnergy();
            // Ensure buttons are re-enabled
            ShowLevelItemInfo();
        }
        else if ((PlayerPrefs.GetInt("Energy") <= 0))
        {
            Main_Menu_Manager_Script._instance.Open_Panel_Energy();
            gameObject.SetActive(false);
        }
    }

    public void Next_Page()
    {
        if (selector_Curent_Page < selector_Total_Page - 1)
        {
            selector_Curent_Page++;
            ShowLevelItemInfo();
        }
    }

    public void Previous_Page()
    {
        if (selector_Curent_Page > 0)
        {
            selector_Curent_Page--;
            ShowLevelItemInfo();
        }
    }
}
