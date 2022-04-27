using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//Class to control the graphics quality of the game
public class QualityController : BaseBehaviour
{
    //Enum to store levels of quality
    [System.Serializable]
    public enum QualityLevel
    {
        Fast,
        Medium,
        High,
        Ultra
    }

    //Refrence to the post processing layer and profile
    [SerializeField] private PostProcessLayer post;
    [SerializeField] private PostProcessProfile profile;

    //Method called on scene load
    private IEnumerator Start()
    {
        //Wait until the save has been loaded
        yield return new WaitUntil(() => Serializer.Instance.activeData != null);
        yield return new WaitUntil(() => Serializer.Instance.activeData.settings != null);

        //Else Set the quality to the saved quality
        SetQuality(Serializer.Instance.activeData.settings.qualityLevel);
    }

    //Method called to change the graphics quality level
    public void SetQuality(QualityLevel level)
    {
        //Gets the bloom from the profile
        Bloom bloom = profile.GetSetting<Bloom>();

        //Sets the max fps to 1024
        Application.targetFrameRate = 1024;

        //Switch with a case for each quality level
        switch (level)
        {
            //Triggers if the quality is fast
            case QualityLevel.Fast:
                //Disables post processing
                post.enabled = false;
                break;

            //Triggers if the quality is medium
            case QualityLevel.Medium:
                //Disables smaa, enables bloom on fast mode
                post.enabled = true;
                post.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Low;
                bloom.enabled.value = true;
                bloom.fastMode.value = true;
                bloom.intensity.value = 7;
                break;

            //Triggers if the quality is high
            case QualityLevel.High:
                //Sets smaa to medium quality, enables bloom
                post.enabled = true;
                post.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Medium;
                bloom.enabled.value = true;
                bloom.fastMode.value = false;
                bloom.intensity.value = 7;
                break;

            //Triggers if the quality is ultra
            case QualityLevel.Ultra:
                //Sets smaa to high quality, enables bloom
                post.enabled = true;
                post.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.High;
                bloom.enabled.value = true;
                bloom.fastMode.value = false;
                bloom.intensity.value = 7;
                break;
        }

        //Assigns the quality level to the save data after it is not 
        StartCoroutine(WaitToAssignQuality(level));
    }

    //Method used to assign quality after the saved data have been loaded
    private IEnumerator WaitToAssignQuality(QualityLevel level)
    {
        //Waits for the saved data
        yield return new WaitUntil(() => Serializer.Instance.activeData != null);

        //Sets the level to the saved data
        Serializer.Instance.activeData.settings.SetQualityLevel(level);
    }

    //Method to return the default quality for a give platform, if windows set to ultra, if android set to medium
    public static QualityLevel DefaultQualityLevel(Gamemanager.Platform platform) =>
        (platform == Gamemanager.Platform.Windows || platform == Gamemanager.Platform.Debug) ?
            QualityLevel.Ultra : QualityLevel.High;
}
