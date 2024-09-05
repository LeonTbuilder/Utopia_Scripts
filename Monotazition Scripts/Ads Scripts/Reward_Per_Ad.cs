using UnityEngine;
using UnityEngine.Advertisements;

public class Reward_Per_Ad : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static Reward_Per_Ad Instance { get; private set; }

    [SerializeField] private string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] private string _iOSAdUnitId = "Rewarded_iOS";

    private string _adUnitId;
    private bool _adLoaded = false;

    private enum AdType { HINT, GEM, ENERGY }
    private AdType currentAdType;

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
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSAdUnitId : _androidAdUnitId;
        InitializeAds();
    }

    private void InitializeAds()
    {
        string gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? "YOUR_IOS_GAME_ID" : "YOUR_ANDROID_GAME_ID";
        if (Advertisement.isInitialized && Advertisement.isSupported)
        {
            LoadAd();
        }
        else
        {
            Advertisement.Initialize(gameId, true, new UnityAdsInitializationListener(this));
        }
    }

    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    public void Show_Hint_Ad()
    {
        currentAdType = AdType.HINT;
        ShowAd();
    }

    public void Show_Gem_Ad()
    {
        currentAdType = AdType.GEM;
        ShowAd();
    }

    public void Show_Energy_Ad()
    {
        currentAdType = AdType.ENERGY;
        ShowAd();
    }

    private void ShowAd()
    {
        if (_adLoaded)
        {
            Debug.Log("Showing Ad: " + _adUnitId);
            Advertisement.Show(_adUnitId, this);
        }
        else
        {
            Debug.Log("Ad not ready. Please wait.");
            LoadAd(); // Try loading the ad again
        }
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId.Equals(_adUnitId))
        {
            Debug.Log("Ad Loaded: " + adUnitId);
            _adLoaded = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        _adLoaded = false;
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("Started showing ad: " + adUnitId);
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log("Ad clicked: " + adUnitId);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Reward the user here.
            RewardUser();
            // Load another ad for future use
            _adLoaded = false;
            LoadAd();
        }
    }

    private void RewardUser()
    {
        // Increment AdsWatched PlayerPrefs
        int adsWatched = PlayerPrefs.GetInt("AdsWatched", 0);
        PlayerPrefs.SetInt("AdsWatched", adsWatched + 1);

        switch (currentAdType)
        {
            case AdType.HINT:
                RewardHint();
                break;
            case AdType.GEM:
                RewardGems();
                break;
            case AdType.ENERGY:
                RewardEnergy();
                break;
        }
    }

    private void RewardHint()
    {
        Debug.Log("Rewarding user with a hint.");
        HintManager.Instance.OnAdWatched();
    }

    private void RewardGems()
    {
        Debug.Log("Rewarding user with gems.");
        int currentGems = PlayerPrefs.GetInt("Gem", 0);
        currentGems += 150;
        PlayerPrefs.SetInt("Gem", currentGems);
        Main_Menu_Manager_Script._instance.Update_Text_Gems();
        UI_inGame_Manager_Script._instance.UpdateGemsTexts(currentGems.ToString());
    }

    private void RewardEnergy()
    {
        Debug.Log("Rewarding user with energy.");
        int currentEnergy = PlayerPrefs.GetInt("Energy", 0);
        int maxEnergy = PlayerPrefs.GetInt("maxEnergy", 5);
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += 1;
            PlayerPrefs.SetInt("Energy", currentEnergy);

            Energy_Manager_Script._instance.UpdateEnergy(currentEnergy);
            Main_Menu_Manager_Script._instance.Update_Energy(currentEnergy);
            UI_inGame_Manager_Script._instance.Update_Energy(currentEnergy);
        }
    }

}

public class UnityAdsInitializationListener : IUnityAdsInitializationListener
{
    private Reward_Per_Ad _rewardPerAd;

    public UnityAdsInitializationListener(Reward_Per_Ad rewardPerAd)
    {
        _rewardPerAd = rewardPerAd;
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        _rewardPerAd.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
