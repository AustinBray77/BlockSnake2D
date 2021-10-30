using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class used to store more animations
public static class AnimationPlus
{
    //Stores fps and wait time
    public static int fps = 120;
    public static float waitTime => 1 / fps;

    //Method to fade an image to a color
    public static IEnumerator FadeToColor(Image img, Color endColor, float time, bool stateAfterCall = true)
    {
        //Stores the total frams which need to be counted and the frames which have been counted
        float totalFrames = fps * time;
        float framesCounted = 0;

        //Stores the amount of color which the image should be changed by each frame
        Color differencePerFrame = (endColor - img.color) / totalFrames;

        //Loops while frames still need to be counted
        while(framesCounted < totalFrames)
        {
            //Changes the images color
            img.color += differencePerFrame;

            //Increments the frame counter and waits the required amount of time
            framesCounted++;
            yield return new WaitForSeconds(waitTime);
        }

        //Sets the image color to the final color, in case it is off by a bit
        img.color = endColor;

        //Sets the object to active if it is supposed to be
        img.gameObject.SetActive(stateAfterCall);
    }

    //Method to animate a level bar object
    public static IEnumerator AnimateLevelBar(UI.LevelBar levelBar, int lowerLevel, float endVal, float time)
    {
        //Stores the total frams which need to be counted and the frames which have been counted
        float totalFrames = fps * time;
        float framesCounted = 0;

        //Stores the amount which the levelBar should be changed by each frame
        float differencePerFrame = (endVal - levelBar.levelBar.value) / totalFrames;

        //Loops while frames still need to be counted
        while (framesCounted < totalFrames)
        {
            //Changes the level bar value
            levelBar.levelBar.value += differencePerFrame;
            
            //Triggers if the player should level up and the level has to change on the level bar
            if(levelBar.levelBar.value >= levelBar.levelBar.maxValue)
            {
                //Inccrements lower level and assigns the values of the level bar
                lowerLevel++;
                levelBar.levelBar.minValue = Level.LevelToXP(lowerLevel);
                levelBar.levelBar.maxValue = Level.LevelToXP(lowerLevel + 1);
                levelBar.levelBar.value = Level.LevelToXP(lowerLevel);
                levelBar.lowerLevel.text = lowerLevel.ToString();
                levelBar.higherLevel.text = (lowerLevel + 1).ToString();
            }

            //Increments the frame counter and waits the required amount of time
            framesCounted++;
            yield return new WaitForSeconds(waitTime);
        }

        //Sets the image color to the final value, in case it is off by a bit
        levelBar.levelBar.value = endVal;
    }

    //Method called to fade text in or out (used in tutorial)
    public static IEnumerator FadeText(TextMeshProUGUI text, float time, bool fadeOut = true)
    {
        //Stores the total frams which need to be counted and the frames which have been counted
        float totalFrames = fps * time;
        float framesCounted = 0;

        //Stores the original text color
        Color startColor = text.color;

        //Stores the amount which the levelBar should be changed by each frame
        float differencePerFrame = 1 / totalFrames * (fadeOut ? -1 : 1);

        while(framesCounted < totalFrames)
        {
            //Changes the opacity of the text
            text.color = text.color + new Color(0, 0, 0, differencePerFrame);

            //Increments the frame counter and waits the required amount of time
            framesCounted++;
            yield return new WaitForSeconds(waitTime);
        }

        //Sets the text color to the final value, in case it is off by a bit
        text.color = startColor + new Color(0, 0, 0, (fadeOut ? -1 : 1));
    }
}
