using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System;
using System.Reflection;
using System.Collections;

//Class containing general processing functions
public class Functions : SingletonDD<Functions>
{
    //Stores the current time from the server
    public static DateTime CurrentTime { get; private set; }

    //Stores if currently attempting to fetch the date time from the server
    public static bool FetchingDateTime { get; private set; }

    //Method to try and convert a value to another type
    public static U TryGenericConversion<T, U>(T val)
    {
        //Gets default value for output
        U output = default(U);

        //Error handling
        try
        {
            //Change the type to the inputted type
            output = (U)Convert.ChangeType(val, typeof(U));
        }
        //Catch any exceptions
        catch (Exception e)
        {
            //Log the exception as a warning
            Debug.LogWarning("Error Caught When Trying Generic Conversion: " + e.ToString());
        }

        //Return the convertted type
        return output;
    }

    //Method to convert an array to a string
    public static string ArrayToString<T>(T[] vals, string seperator = " ")
    {
        //Stores the output
        string output = "";

        //Loops through each value in the array
        foreach (T val in vals)
        {
            //Converts the value to a string and adds it with the spereator
            output += val.ToString() + seperator;
        }

        //Returns the output with the last character removed
        return output.Substring(0, output.Length - seperator.Length);
    }

    //Method that gets a random item from the array
    public static T RandomArrItem<T>(T[] vals) =>
        //Gets a value at a random index
        vals[UnityEngine.Random.Range(0, vals.Length)];

    //Method that returns if a position is in a box bounds
    public static bool PositionIsInBounds2D(Vector2 position, Vector2 topRight, Vector2 bottomLeft) =>
        position.x <= topRight.x && position.x >= bottomLeft.x && position.y <= topRight.y && position.y >= bottomLeft.y;

    //Method that returns if the user is clicking
    public static bool UserIsClicking(int mouseButton = 0) =>
        Input.GetMouseButton(mouseButton) || Input.touchCount > 0;

    //Coroutine to play an audio source once
    public static IEnumerator PlayAudioOnce(AudioSource audio)
    {
        //Starts the audio source
        audio.Play();

        //Waits the length of the audio clip
        yield return new WaitForSeconds(audio.clip.length);

        //Stops the audio source
        audio.Stop();
    }

    //Method to convert a vector3 to a string
    public static string Vector3ToString(Vector3 vec) =>
        vec.x + " " + vec.y + " " + vec.z;

    //Method to deep clone a vector3
    public static Vector3 CopyVector3(Vector3 vec) =>
        new Vector3(vec.x, vec.y, vec.z);

    //Coroutine to update the current time 
    public static IEnumerator UpdateCurrentTime()
    {
        //Flags that the time is currently being fetched
        FetchingDateTime = true;

        //Url for the server with the time
        string url = "https://blcksnktime.000webhostapp.com/servertime.php";

        //Intializes the request with the url
        UnityWebRequest webReq = UnityWebRequest.Get(url);

        //Sets the timeout to 3 secs
        webReq.timeout = 3;

        //Sends the request
        yield return webReq.SendWebRequest();

        //Trigger if the result failed
        if (UnityWebRequest.Result.ConnectionError == webReq.result)
        {
            //Set the time to 1/1/1970 0:00
            CurrentTime = new DateTimeOffset().DateTime;
        }
        //Else if the result was successful
        else
        {
            //Gets the time from the website in string form
            string time = webReq.downloadHandler.text;

            //Triggers if the date time is able to parse
            if (DateTime.TryParse(time, out DateTime serverTime))
            {
                //Sets the time
                CurrentTime = serverTime;
            }
        }

        //Flags that the time has currently
        FetchingDateTime = false;
    }

    //Method to convert the current time to miliseconds
    public static long CurrentTimeInMillis() =>
        new DateTimeOffset(CurrentTime).ToUnixTimeMilliseconds();

    //Method to get the current time zone offset
    public static long TimezoneOffset() =>
        ((System.DateTimeOffset.Now.ToUnixTimeMilliseconds() / 3600000) - (CurrentTimeInMillis() / 3600000)) % 24;

    //Method to get the days since unix in millis
    public static long DaysSinceUnixFromMillis(long millis) =>
        millis / 86400000;

    //Method to get current time in millis, in the current time zone
    public static long CurrentMillisInTimeZone() =>
        CurrentTimeInMillis() + (3600000 * TimezoneOffset());

    //Method to round a float to a certain amount decimal places
    public static float RoundToPlaces(float f, int places) =>
        Mathf.Round(f * Mathf.Pow(10, places)) / Mathf.Pow(10, places);

    //Method to get properties of a given type name
    public static PropertyInfo[] GetProperties(string type) =>
        Type.GetType(type).GetProperties();
}