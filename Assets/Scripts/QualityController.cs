using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//Class to control the graphics quality of the game
public class QualityController : MonoBehaviour
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

    //Refrence to the post processing layer
    [SerializeField] private PostProcessLayer post;
    [SerializeField] private PostProcessProfile profile;

    //Method called on scene load
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Serializer.activeData != null);

        //Else Set the quality to the saved quality
        SetQuality(Serializer.activeData.settings.qualityLevel);
    }

    //Method called to change the graphics quality level
    public void SetQuality(QualityLevel level)
    {
        //Gets the bloom from the profile
        Bloom bloom = profile.GetSetting<Bloom>();

        //Sets the max fps to 1024
        Application.targetFrameRate = 1024;

        //Switch with a case for each value
        switch (level)
        {
            //Triggers if the quality is fast
            case QualityLevel.Fast:
                //Disables smaa and bloom
                //post.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Low;
                //bloom.enabled.value = false;
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

    private IEnumerator WaitToAssignQuality(QualityLevel level)
    {
        yield return new WaitUntil(() => { return Serializer.activeData != null; });
        Serializer.activeData.settings.SetQualityLevel(level);
    }

    public static QualityLevel DefaultQualityLevel(Gamemode.Platform platform) =>
        (platform == Gamemode.Platform.Windows || platform == Gamemode.Platform.Debug) ?
            QualityLevel.Ultra : QualityLevel.Fast;
}
