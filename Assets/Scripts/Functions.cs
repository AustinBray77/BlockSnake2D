using UnityEngine;
using System.Collections;

public static class Functions
{
    public static string ArrayToString<T>(T[] vals, string seperator = " ")
    {
        string output = "";

        foreach (T val in vals)
        {
            output += val.ToString() + seperator;
        }

        return output.Substring(0, output.Length - seperator.Length);
    }

    public static bool PositionIsInBounds2D(Vector2 position, Vector2 topRight, Vector2 bottomLeft) =>
        position.x <= topRight.x && position.x >= bottomLeft.x && position.y <= topRight.y && position.y >= bottomLeft.y;

    public static bool UserIsClicking(int mouseButton = 0) =>
        Input.GetMouseButton(mouseButton) || Input.touchCount > 0;

    public static IEnumerator PlayAudioOnce(AudioSource audio)
    {
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
        audio.Stop();
    }

    public static string Vector3ToString(Vector3 vec) =>
        vec.x + " " + vec.y + " " + vec.z;

    public static Vector3 CopyVector3(Vector3 vec) =>
        new Vector3(vec.x, vec.y, vec.z);
}