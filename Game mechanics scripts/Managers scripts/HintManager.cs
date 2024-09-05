using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    public Button watchAdButton;
    public TMP_Text adCountText;
    public GameObject hand_PF;
    public GameObject maxAdsWatchedPanel;
    public Button closeMaxAdsWatchedPanelButton;

    private int adsWatched = 0;
    private const int maxAdsPerLevel = 2;
    private GameObject currentHint;
    private List<GameObject> hintObjects;
    private int currentHintIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        watchAdButton.onClick.AddListener(OnWatchAdButtonClicked);
        closeMaxAdsWatchedPanelButton.onClick.AddListener(OnCloseMaxAdsWatchedPanelButtonClicked);
        maxAdsWatchedPanel.SetActive(false);
        UpdateAdCountText();
    }

    public void SetHintObjects(List<GameObject> objects)
    {
        hintObjects = objects;
        currentHintIndex = 0;
    }

    private void OnWatchAdButtonClicked()
    {
        if (adsWatched < maxAdsPerLevel)
        {
            Reward_Per_Ad.Instance.Show_Hint_Ad();
        }
        else
        {
            maxAdsWatchedPanel.SetActive(true);
        }
    }

    public void OnAdWatched()
    {
        adsWatched++;
        UpdateAdCountText();
        ShowHint();
        UI_inGame_Manager_Script._instance.Close_Hint_Panel();
    }

    private void ShowHint()
    {
        if (hintObjects == null || hintObjects.Count == 0)
        {
            Debug.LogWarning("Hint objects list is empty or not set.");
            return;
        }

        while (currentHintIndex < hintObjects.Count && (hintObjects[currentHintIndex] == null || !hintObjects[currentHintIndex].activeInHierarchy))
        {
            currentHintIndex++;
        }

        if (currentHintIndex < hintObjects.Count)
        {
            if (currentHint != null)
            {
                Destroy(currentHint);
            }

            GameObject hintObject = hintObjects[currentHintIndex];
            if (hintObject != null && hintObject.activeInHierarchy)
            {
                Vector3 hintPosition = hintObject.transform.position;

                currentHint = Instantiate(hand_PF, hintPosition, Quaternion.identity);

                currentHint.transform.SetParent(hintObject.transform, false);

                currentHint.transform.localPosition = Vector3.zero;

                Pin_Engine_Script pinEngine = hintObject.GetComponent<Pin_Engine_Script>();
                pinEngine?.ActivateHint(currentHint);

                Debug.Log($"Hint displayed on object: {hintObject.name} at index: {currentHintIndex}");
                currentHintIndex++;
            }
            else
            {
                Debug.LogWarning("No valid hint object found.");
            }
        }
        else
        {
            Debug.LogWarning("All hint objects have been used or are inactive.");
        }
    }


    private void OnCloseMaxAdsWatchedPanelButtonClicked()
    {
        maxAdsWatchedPanel.SetActive(false);
    }

    private void UpdateAdCountText()
    {
        adCountText.text = $"{adsWatched}/{maxAdsPerLevel}";
    }
}
