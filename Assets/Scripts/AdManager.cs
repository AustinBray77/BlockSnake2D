using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    //Instance variables
    private string gameID = "4271826", adID = "";
    private bool testMode;
    private bool isValid = true;

    //Method called on scene load
    private void Start()
    {
        //Deactivates the object if the platform is windows
        if (Gamemode.platform == Gamemode.Platform.Windows)
        {
            gameObject.SetActive(false);
            return;
        }

        //Determines if the ad manager should be in test mode
        testMode = Gamemode.platform == Gamemode.Platform.Debug;

        //Determinds the ad ID and game ID
        if (Gamemode.platform == Gamemode.Platform.Android || Gamemode.platform == Gamemode.Platform.Debug)
        {
            //gameID = "415970920840";
            gameID = "4271826";
            adID = "Rewarded_Android";
        }
        else if (Gamemode.platform == Gamemode.Platform.IOS)
        {
            gameID = "4271827";
            adID = "Rewarded_iOS";
        }
        else if (!testMode)
        {
            isValid = false;
            Debug.Log("Invalid Platform");
            return;
        }

        //Initializes the advertisment
        Advertisement.Initialize(gameID, testMode);
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
    public void ShowAd()
    {
        // Then show the ad:
        Advertisement.Show(adID, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(adID) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            Refrence.deathUI.OnRespawn();
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
}
