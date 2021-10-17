using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public IEnumerator ClickWithFade(System.Action action, float seconds, Image fadePanel)
    {
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 1), seconds));
        yield return new WaitForSeconds(seconds);

        action.Invoke();

        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), seconds, false));
    }
}
