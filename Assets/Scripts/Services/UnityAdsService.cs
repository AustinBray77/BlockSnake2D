using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

//Class to control showing unity ads to the user
public class UnityAdsService : SingletonDD<UnityAdsService>,
                               IUnityAdsLoadListener,
                               IUnityAdsShowListener
{
    //Stores if the service is in test mode
    private bool _testMode;
    private bool _isValid = true;

    //Stores the action to call after the next ad showing
    private System.Action _actionAfterAd;

    //Stores the id of the current ad
    private string _adID;

    //Assigns ad ids for android platform
#if UNITY_ANDROID
    private static string s_gameID = "4271826";
    private static string s_rewarded_AD = "Rewarded_Android";
    private static string s_intersertial_AD = "Interstitial_Android";
    //Assigns ad ids for IOS platform
#elif UNITY_IOS
    private static string s_gameID = "4271827";
    private static string s_rewarded_AD = "Rewarded_IOS";
    private static string s_intersertial_AD = "Interstitial_IOS";
#endif

    //Method called on scene load
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public void Start()
    {
        //Intializes the advertizesment service
        Advertisement.Initialize(s_gameID, false);
    }

    //Method called to load an ad
    public void LoadAd()
    {
        //Loads the ad of the set id
        Log("Loading Ad: " + _adID);
        Advertisement.Load(_adID, this);
    }

    //Method called when ad is loaded
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        //Log that the ad was loaded
        Log("Ad Loaded: " + adUnitId);
    }

    //Method to show an ad
    private void ShowAd()
    {
        //Shows the ad of the set id
        Advertisement.Show(_adID, this);
    }

    //Method called when the ad is finished showing
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        //Trigger is the ad was completed or the ad was skipped and is an intersertial ad
        if ((adUnitId.Equals(_adID) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            || (_adID == s_intersertial_AD && showCompletionState.Equals(UnityAdsCompletionState.SKIPPED)))
        {
            //Invokes the method to be called after the ad
            _actionAfterAd.Invoke();

            //Resets ad id and after ad method
            _actionAfterAd = null;
            _adID = "";
        }
    }

    //Method called when the ad fails to load
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        //Logs that the ad load failed
        Log($"Error loading Ad Unit {adUnitId}: {error} - {message}");
    }

    //Method called when the ad fails to show
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        //Logs that the ad failed to show
        Log($"Error showing Ad Unit {adUnitId}: {error} - {message}");
    }

    //Implementations for interface
    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    /*
    void OnDestroy()
    {
        // Clean up the button listeners:
        //_showAdButton.onClick.RemoveAllListeners();
    }*/

    //Method for showing a rewarded ad and then invoking a method
    public void ShowRewardedAdThenCall(System.Action action)
    {
        //Assigns the action and ad id
        _actionAfterAd = action;
        _adID = s_rewarded_AD;

        //Shows the ad
        ShowAd();
    }

    //Method for showing an intersertial ad and then invoking a method
    public void ShowIntersertialAdThenCall(System.Action action)
    {
        //Assigns the action and ad id
        _actionAfterAd = action;
        _adID = s_intersertial_AD;

        //Shows the ad
        ShowAd();
    }
}