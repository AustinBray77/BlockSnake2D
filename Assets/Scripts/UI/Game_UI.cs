using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the game UI
public class Game_UI : UI
{
    //Object refrence to the controls
    [SerializeField] private GameObject controls;

    //Called when the scene is loaded
    private void Start()
    {
        //Deactivates the on screen controls if the user is playing the windows version of the game
        controls.SetActive(Gamemode.platform != Gamemode.Platform.Windows);        
    }
}
