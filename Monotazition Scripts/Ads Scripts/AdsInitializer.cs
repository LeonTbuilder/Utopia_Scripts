using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    private string _androidGameId = "5663222";
    private string _iOSGameId = "5663223";
    [SerializeField] private bool _testMode = true;
    private string _gameId;
    private bool isInitialized = false;

    public event Action OnInitializationCompleteEvent;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSGameId : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        isInitialized = true;
        OnInitializationCompleteEvent?.Invoke();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }
}
