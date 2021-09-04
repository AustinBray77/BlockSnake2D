using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugExtras : MonoBehaviour
{
    void Update()
    {
        if (Refrence.player == null)
            return;

        if(Input.GetKeyDown(KeyCode.J))
        {
            Refrence.player.RemoveSegments(1);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Refrence.player.RemoveSegments(5);
        }
    }
}
