using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax_Object : MonoBehaviour
{
    public int layer;

    private static float speed = 2;

    //private void Awake()
    //{
    //    tag = "Parallax";
    //}

    private void Update()
    {
        if(!Player.isAtFinish && !Player.isDead)
            transform.position += new Vector3(-speed / (layer + 1), 0) * Time.deltaTime;
    }
}
