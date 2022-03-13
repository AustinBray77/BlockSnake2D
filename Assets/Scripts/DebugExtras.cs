using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used for debugging
public class DebugExtras : BaseBehaviour
{
    //Method called on each frame
    void FixedUpdate()
    {
        //Returns if the platform is not set to debug
        if (Gamemode.platform != Gamemode.Platform.Debug)
            return;

        //Triggers if the current scene is main (play will not be null)
        if (Reference.player != null)
        {
            //If the j key is hit, 1 segment is removed from the player
            if (Input.GetKeyDown(KeyCode.J))
            {
                Reference.player.RemoveSegments(1);
                Log("1 Segment Removed");
            }

            //If the k key is hit, 5 segments are removed from the player
            if (Input.GetKeyDown(KeyCode.K))
            {
                Reference.player.RemoveSegments(5);
                Log("5 Segment Removed");
            }

            //If the l key is hit, 1000 xp is added to the player
            if (Input.GetKeyDown(KeyCode.L))
            {
                Player.level.AddXP(1000);
                Log("1000 XP Added");
            }

            //If the g key is hit, 10 gears are added to the player
            /*if (Input.GetKeyDown(KeyCode.G))
            {
                Player.gearCount += 10;
                Log("10 Gears Added");
            }*/

            if (Input.GetKeyDown(KeyCode.B))
            {
                Reference.player.AddScore(5);
            }

            //If the h key is hit, 1 shield is added to the player
            if (Input.GetKeyDown(KeyCode.H))
            {
                Reference.player.AddShields(1);
                Log("1 Shield Added");
            }

            //If the v key is hit, 1 shield is used
            if (Input.GetKeyDown(KeyCode.V))
            {
                Reference.player.UseShield();
                Log("1 Shield Used");
            }

            //if the ; key is hit level up the slowdown powerup
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                Reference.cardTypes[4].Call();
            }
        }

        //If the R key is hit, the save game is reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            Serializer.Instance.ResetData();
            Serializer.Instance.SaveData();
            Log("Save Reset");
        }
    }
}
