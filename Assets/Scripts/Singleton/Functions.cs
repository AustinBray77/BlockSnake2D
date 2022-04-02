using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System;
using System.Reflection;
using System.Collections;

public class Functions : SingletonDD<Functions>
{
    public static DateTime CurrentTime { get; private set; }
    public static bool FetchingDateTime { get; private set; }

    public static U TryGenericConversion<T, U>(T val)
    {
        U output = default(U);

        try
        {
            output = (U)Convert.ChangeType(val, typeof(U));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error Caught When Trying Generic Conversion: " + e.ToString());
        }

        return output;
    }

    public static string ArrayToString<T>(T[] vals, string seperator = " ")
    {
        string output = "";

        foreach (T val in vals)
        {
            output += val.ToString() + seperator;
        }

        return output.Substring(0, output.Length - seperator.Length);
    }

    public static T RandomArrItem<T>(T[] vals) =>
        vals[UnityEngine.Random.Range(0, vals.Length)];

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

    public static IEnumerator UpdateCurrentTime()
    {
        FetchingDateTime = true;

        string url = "https://blcksnktime.000webhostapp.com/servertime.php";

        UnityWebRequest webReq = UnityWebRequest.Get(url);

        webReq.timeout = 3;

        yield return webReq.SendWebRequest();

        if (UnityWebRequest.Result.ConnectionError == webReq.result)
        {
            CurrentTime = new DateTimeOffset().DateTime;
        }
        else
        {
            string time = webReq.downloadHandler.text;

            if (DateTime.TryParse(time, out DateTime serverTime))
            {
                CurrentTime = serverTime;
            }
        }

        FetchingDateTime = false;
    }

    public static long CurrentTimeInMillis() =>
        new DateTimeOffset(CurrentTime).ToUnixTimeMilliseconds();

    public static long TimezoneOffset()
    {
        return ((System.DateTimeOffset.Now.ToUnixTimeMilliseconds() / 3600000) - (CurrentTimeInMillis() / 3600000)) % 24;
    }

    public static long DaysSinceUnixFromMillis(long millis) =>
        millis / 86400000;

    public static long CurrentMillisInTimeZone() =>
        CurrentTimeInMillis() + (3600000 * TimezoneOffset());

    public static float RoundToPlaces(float f, int places) =>
        Mathf.Round(f * Mathf.Pow(10, places)) / Mathf.Pow(10, places);

    public static PropertyInfo[] GetProperties(string type) =>
        Type.GetType(type).GetProperties();
}