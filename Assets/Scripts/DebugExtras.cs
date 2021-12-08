using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used for debugging
public class DebugExtras : MonoBehaviour
{
    //Method called on each frame
    void FixedUpdate()
    {
        //Returns if the platform is not set to debug
        if (Gamemode.platform != Gamemode.Platform.Debug)
            return;

        //Triggers if the current scene is main (play will not be null)
        if (Refrence.player != null)
        {
            //If the j key is hit, 1 segment is removed from the player
            if (Input.GetKeyDown(KeyCode.J))
            {
                Refrence.player.RemoveSegments(1);
                Debug.Log("1 Segment Removed");
            }

            //If the k key is hit, 5 segments are removed from the player
            if (Input.GetKeyDown(KeyCode.K))
            {
                Refrence.player.RemoveSegments(5);
                Debug.Log("5 Segment Removed");
            }

            //If the l key is hit, 1000 xp is added to the player
            if (Input.GetKeyDown(KeyCode.L))
            {
                Player.level.AddXP(1000);
                Debug.Log("1000 XP Added");
            }

            //If the g key is hit, 10 gears are added to the player
            if(Input.GetKeyDown(KeyCode.G))
            {
                Player.gearCount += 10;
                Debug.Log("10 Gears Added");
            }

            //If the h key is hit, 1 shield is added to the player
            if (Input.GetKeyDown(KeyCode.H))
            {
                Refrence.player.AddShields(1);
                Debug.Log("1 Shield Added");
            }

            //If the v key is hit, 1 shield is used
            if (Input.GetKeyDown(KeyCode.V))
            {
                Refrence.player.UseShield();
                Debug.Log("1 Shield Used");
            }
        }

        //If the R key is hit, the save game is reset
        if(Input.GetKeyDown(KeyCode.R))
        {
            Serializer.ResetData();
            Serializer.SaveData();
            Debug.Log("Save Reset");
        }
    }
}
