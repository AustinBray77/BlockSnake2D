using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    //Instance variables
    private bool testMode;
    private bool isValid = true;
    private System.Action actionAfterAd;
    private string adID;

#if UNITY_ANDROID
    private string gameID = "4271826";
    private string rewarded_AD = "Rewarded_Android";
    private string intersertial_AD = "Interstitial_Android";
#elif UNITY_IOS
    private string gameID = "4271827";
    private string rewarded_AD = "Rewarded_IOS";
    private string intersertial_AD = "Interstitial_IOS";
#endif

    //Method called on scene load
    private void Start()
    {
        //Deactivates the object if the platform is windows
        if (Gamemode.platform == Gamemode.Platform.Windows)
        {
            gameObject.SetActive(false);
            return;
        }

        //Initializes the advertisment
        Advertisement.Initialize(gameID, false);
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + adID);
        Advertisement.Load(adID, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
    }

    // Implement a method to execute when the user clicks the button.
    private void ShowAd()
    {
        // Then show the ad:
        Advertisement.Show(adID, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(adID) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            actionAfterAd.Invoke();
            actionAfterAd = null;
            adID = "";
        }
        else if (adID == intersertial_AD && showCompletionState.Equals(UnityAdsCompletionState.SKIPPED))
        {
            actionAfterAd.Invoke();
            actionAfterAd = null;
            adID = "";
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error} - {message}");
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
        actionAfterAd = action;
        adID = rewarded_AD;
        ShowAd();
    }

    public void ShowIntersertialAdThenCall(System.Action action)
    {
        actionAfterAd = action;
        adID = intersertial_AD;
        ShowAd();
    }
}
