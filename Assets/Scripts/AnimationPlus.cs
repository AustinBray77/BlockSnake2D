using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class used to store more animations
public class AnimationPlus
{
    //Method to fade an image to a color
    public static IEnumerator FadeToColor(Image img, Color endColor, float time, bool stateAfterCall = true)
    {
        //Stores the amount of color which the image should be changed by each frame
        Color differencePerSecond = (endColor - img.color) / time;

        //Stores the amount of time that has passed
        float timeCount = 0f;

        //Loops while frames still need to be counted
        while (timeCount < time)
        {
            //Gets the time the passed
            float timePassed = Time.deltaTime;

            //Changes the images color
            img.color += differencePerSecond * timePassed;

            //Increments the frame counter and waits the required amount of time
            timeCount += timePassed;
            yield return new WaitForEndOfFrame();
        }

        //Sets the image color to the final color, in case it is off by a bit
        //img.color = endColor;

        //Sets the object to active if it is supposed to be
        img.gameObject.SetActive(stateAfterCall);
    }

    //Method to animate a level bar object
    public static IEnumerator AnimateLevelBar(LevelBar levelBar, int lowerLevel, float endVal, float time)
    {
        //Stores the amount which the levelBar should be changed by each frame
        float differencePerSecond = (endVal - levelBar.levelBar.value) / time;

        //Stores the amount of time that has passed
        float timeCount = 0f;

        //Loops while frames still need to be counted
        while (timeCount < time)
        {
            //Gets the time the passed
            float timePassed = Time.deltaTime;

            //Changes the level bar value
            levelBar.levelBar.value += differencePerSecond * timePassed;

            //Triggers if the player should level up and the level has to change on the level bar
            if (levelBar.levelBar.value >= levelBar.levelBar.maxValue)
            {
                //Inccrements lower level and assigns the values of the level bar
                lowerLevel++;
                levelBar.levelBar.minValue = Level.LevelToXP(lowerLevel);
                levelBar.levelBar.maxValue = Level.LevelToXP(lowerLevel + 1);
                levelBar.levelBar.value = Level.LevelToXP(lowerLevel);
                levelBar.lowerLevel.text = lowerLevel.ToString();
                levelBar.higherLevel.text = (lowerLevel + 1).ToString();

                if (lowerLevel < 100) Death_UI.Instance.TriggerLevelPrompt(lowerLevel - 1);
            }

            //Increments the frame counter and waits the required amount of time
            timeCount += timePassed;
            yield return new WaitForEndOfFrame();
        }

        //Sets the image color to the final value, in case it is off by a bit
        levelBar.levelBar.value = endVal;
    }

    //Method called to fade text in or out (used in tutorial)
    public static IEnumerator FadeText(TextMeshProUGUI text, float time, bool fadeOut = true)
    {
        //Stores the original text color
        Color startColor = text.color;

        //Stores the amount which the levelBar should be changed by each frame
        float differencePerSecond = (fadeOut ? -1 : 1) / time;

        //Stores the amount of time that has passed
        float timeCount = 0f;

        while (timeCount < time)
        {
            //Gets the time the passed
            float timePassed = Time.deltaTime;

            //Changes the opacity of the text
            text.color = text.color + new Color(0, 0, 0, differencePerSecond * timePassed);

            //Increments the frame counter and waits the required amount of time
            timeCount += timePassed;
            yield return new WaitForEndOfFrame();
        }

        //Sets the text color to the final value, in case it is off by a bit
        if (fadeOut) text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }

    //Method called to zoom the camera in or out
    public static IEnumerator ZoomOutOrthagraphic(Camera cam, float time, float endSize)
    {
        //Stores the original orthagraphic size
        float startSize = cam.orthographicSize;

        //Stores the amount which the camera should zoom out perframe
        float differencePerSecond = (endSize - startSize) / time;

        //Stores the amount of time that has passed
        float timeCount = 0f;

        while (timeCount < time)
        {
            //Gets the time the passed
            float timePassed = Time.deltaTime;

            //Changes the opacity of the text
            cam.orthographicSize += differencePerSecond * timePassed;

            //Increments the frame counter and waits the required amount of time
            timeCount += timePassed;
            yield return new WaitForEndOfFrame();
        }

        //Sets the orthagraphic size to the final value, in case it is off by a bit
        //cam.orthographicSize = endSize;
    }
}
