using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//Class to control the graphics quality of the game
public class QualityController : MonoBehaviour
{
    //Refrence to the post processing layer
    [SerializeField] private PostProcessLayer post;

    //Method called on scene load
    private void Start()
    {
        //If the platform is windows or debug the SMAA quality is set to high, else (Android, IOS) it is set to low
        if(Gamemode.platform == Gamemode.Platform.Windows || Gamemode.platform == Gamemode.Platform.Debug)
        {
            post.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.High;
        } else
        {
            post.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Low;
        }
    }
}
