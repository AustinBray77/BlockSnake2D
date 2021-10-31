using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//Class to control the graphics quality of the game
public class QualityController : MonoBehaviour
{
    //Refrence to the post processing layer
    [SerializeField] private PostProcessLayer post;
    [SerializeField] private PostProcessProfile profile;

    //Method called on scene load
    private void Start()
    {
        //If the platform is windows or debug the SMAA quality is set to high, else (Android, IOS) it is set to low
        if(Gamemode.platform == Gamemode.Platform.Windows || Gamemode.platform == Gamemode.Platform.Debug)
        {
            //Increases the postprocessing quality
            post.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.High;
            Bloom bloom = profile.GetSetting<Bloom>();
            bloom.enabled.value = true;
            bloom.fastMode.value = false;
            bloom.intensity.value = 7;
        } else
        {
            //Sets target fps to 60
            Application.targetFrameRate = 60;

            //Lowers the postprocessing quality
            post.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Low;
            profile.GetSetting<Bloom>().enabled.value = false;
            //Bloom bloom = profile.GetSetting<Bloom>();
            //bloom.fastMode.value = true;
            //bloom.intensity.value = 2;
            //bloom.enabled = false;
        }
    }
}
