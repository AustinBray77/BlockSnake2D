using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsService : SingletonDD<UnityAdsService>,
                               IUnityAdsLoadListener,
                               IUnityAdsShowListener
{
    //Instance variables
    private bool _testMode;
    private bool _isValid = true;
    private System.Action _actionAfterAd;
    private string _adID;

#if UNITY_ANDROID
    private static string s_gameID = "4271826";
    private static string s_rewarded_AD = "Rewarded_Android";
    private static string s_intersertial_AD = "Interstitial_Android";
#elif UNITY_IOS
    private static string s_gameID = "4271827";
    private static string s_rewarded_AD = "Rewarded_IOS";
    private static string s_intersertial_AD = "Interstitial_IOS";
#endif

    //Initializes the service
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeUnityService()
    {
        Advertisement.Initialize(s_gameID, false);
    }

    //Method called on scene load
    /*private void Start()
    {
        //Deactivates the object if the platform is windows
        if (Gamemode.platform == Gamemode.Platform.Windows)
        {
            gameObject.SetActive(false);
            return;
        }

        //Initializes the advertisment
        Advertisement.Initialize(gameID, false);
    }*/

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Log("Loading Ad: " + _adID);
        Advertisement.Load(_adID, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Log("Ad Loaded: " + adUnitId);
    }

    // Implement a method to execute when the user clicks the button.
    private void ShowAd()
    {
        // Then show the ad:
        Advertisement.Show(_adID, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adID) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            _actionAfterAd.Invoke();
            _actionAfterAd = null;
            _adID = "";
        }
        else if (_adID == s_intersertial_AD && showCompletionState.Equals(UnityAdsCompletionState.SKIPPED))
        {
            _actionAfterAd.Invoke();
            _actionAfterAd = null;
            _adID = "";
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Log($"Error loading Ad Unit {adUnitId}: {error} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Log($"Error showing Ad Unit {adUnitId}: {error} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        // Clean up the button listeners:
        //_showAdButton.onClick.RemoveAllListeners();
    }

    public void ShowRewardedAdThenCall(System.Action action)
    {
        _actionAfterAd = action;
        _adID = s_rewarded_AD;
        ShowAd();
    }

    public void ShowIntersertialAdThenCall(System.Action action)
    {
        _actionAfterAd = action;
        _adID = s_intersertial_AD;
        ShowAd();
    }
}