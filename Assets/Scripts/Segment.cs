using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Segment : MonoBehaviour
{
    [HideInInspector] public Segment segmentAfter;
    
    private BoxCollider2D _collider;
    private bool canCollide;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        canCollide = false;

        _collider.size = new Vector2(0, 0);
    }

    public void SetMovement(Vector3 previousPosition, Vector3 previousRotation, float turnDelay)
    {
        if(segmentAfter != null)
            StartCoroutine(MoveNext(transform.position - (transform.right * 1.5f), transform.rotation.eulerAngles, turnDelay));

        transform.rotation = Quaternion.Euler(previousRotation);
        transform.position = previousPosition;

        if(!canCollide)
        {
            canCollide = true;
            _collider.size = new Vector2(1, 1);
            _collider.offset = new Vector2(0, 0);
        }
    }

    private IEnumerator MoveNext(Vector3 position, Vector3 rotation, float turnDelay)
    {
        float time = 0;

        while (time < turnDelay)
        {
            time += Time.fixedDeltaTime;

            yield return new WaitForSeconds(Time.fixedDeltaTime);

            if (Player.isAtFinish)
                yield return new WaitUntil(() => { return !Player.isAtFinish; });
        }

        if (segmentAfter != null)
            segmentAfter.SetMovement(position, rotation, turnDelay);

        yield return 0;
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }
}
