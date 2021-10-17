using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class AnimationPlus
{
    public static int fps = 120;
    public static float waitTime => 1 / fps;

    public static IEnumerator FadeToColor(Image img, Color endColor, float time, bool stateAfterCall = true)
    {
        float totalFrames = fps * time;
        float framesCounted = 0;
        Color difference = endColor - img.color;
        Color differencePerFrame = difference / totalFrames;

        while(framesCounted < totalFrames)
        {
            yield return new WaitForSeconds(waitTime);

            img.color += differencePerFrame;

            framesCounted++;
        }

        img.gameObject.SetActive(stateAfterCall);
    }
}
