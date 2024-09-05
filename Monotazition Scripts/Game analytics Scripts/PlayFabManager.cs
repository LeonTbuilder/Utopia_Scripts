using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;
    private PlayFabAuthenticationContext _authContext;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "YOUR_TITLE_ID"; // Replace with your PlayFab Title ID
        }
        LoginWithCustomID();
    }

    private void LoginWithCustomID()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
        _authContext = result.AuthenticationContext;
        // Further processing after successful login
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
    }

    public void SavePlayerData(string key, string value)
    {
        if (_authContext == null)
        {
            Debug.LogError("Authentication context is null. Cannot save data.");
            return;
        }

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { key, value } },
            AuthenticationContext = _authContext
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnError);
    }

    private void OnDataSendSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Player data updated successfully");
    }

    public void GetPlayerData(string key)
    {
        if (_authContext == null)
        {
            Debug.LogError("Authentication context is null. Cannot get data.");
            return;
        }

        var request = new GetUserDataRequest
        {
            PlayFabId = _authContext.PlayFabId,
            AuthenticationContext = _authContext
        };

        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (result.Data != null && result.Data.ContainsKey(key))
            {
                Debug.Log($"{key}: {result.Data[key].Value}");
            }
            else
            {
                Debug.LogWarning("No data found for this key");
            }
        }, OnError);
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error occurred: " + error.GenerateErrorReport());
    }
}
