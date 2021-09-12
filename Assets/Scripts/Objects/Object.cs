using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class Object : MonoBehaviour
{
    public static float speed;
    public static int objectLayer = 3;
    public int maxOnScreen;

    private void Awake()
    {
        gameObject.tag = "Object";
        GetComponent<SpriteRenderer>().sortingOrder = objectLayer;
        ObjAwake();
    }

    private void Update()
    {
        if (!Player.isDead && !Player.isAtFinish)
        {
            transform.position += -transform.right * speed * Time.deltaTime;
        }

        ObjUpdate();
    }

    internal virtual void ObjAwake() { }
    internal virtual void ObjUpdate() { }
}
