using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using UnityEngine;
using EncryptorLIB;

//Class to contorl the serilization and saving of the gane data
public class Serializer : SingletonDD<Serializer>
{
    //Key for encryption
    public static string Key => SystemInfo.deviceUniqueIdentifier;
    //Variable to store the game's data
    public Save_Data activeData { get; private set; } = null;

    //Property to calculate the data path to save to
    public static string FileName =>
        Application.persistentDataPath + "/blcksnk.bin";

    //Method called to save the data to the hardware
    public IEnumerator SaveData()
    {
        Log("Saving Data...");

        if (Gamemanager.Instance.CurrentPlatform == Gamemanager.Platform.Android)
        {
            //Try to sign in to google play services
            yield return StartCoroutine(PlayGamesService.Instance.SignIn(false));
        }

        //Gets all properties of save data
        PropertyInfo[] properties = Functions.GetProperties("Save_Data");
        int x = 0;

        string totalData = "";

        //Saves each property to a different file
        foreach (PropertyInfo info in properties)
        {
            var value = info.GetValue(activeData);
            string data;
            Type type = value.GetType();

            if (type.IsArray)
            {
                type = value.GetType().GetElementType();
                MethodInfo method = typeof(Functions).GetMethod("ArrayToString").MakeGenericMethod(new Type[] { type });
                data = (string)method.Invoke(null, new object[] { value });
            }
            else
            {
                data = value.ToString();
            }

            data = /*info.Name + ":" +*/ data + "\n";

            //Creates file at persistent data path
            FileStream file = File.Create(Application.persistentDataPath + x + ".bin");

            //Encrypts the data
            data = Encryptor.EncryptWithKey(data, Key);
            totalData += data;

            x++;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(totalData);

        GameMetadata metadata = new GameMetadata(true, FileName, "Lastest Save", "", new TimeSpan(0, 0, 0), Functions.CurrentTime);

        if (Gamemanager.Instance.CurrentPlatform == Gamemanager.Platform.Android)
        {
            PlayGamesService.Instance.SaveGame(metadata, bytes);
        }

        if (File.Exists(FileName)) File.Delete(FileName);
    }

    //Method called to load the data from the hardware
    public IEnumerator LoadData()
    {
        Log("Loading Data...", true);

        byte[] bytes = null;

        if (Gamemanager.Instance.CurrentPlatform == Gamemanager.Platform.Android)
        {
            yield return StartCoroutine(PlayGamesService.Instance.LoadGame(FileName));
            bytes = PlayGamesService.Instance.LastSave;
        }
        else if (File.Exists(FileName))
        {
            bytes = File.ReadAllBytes(FileName);
        }

        //Triggers if the file exists at the persistent data path
        if (bytes != null)
        {
            Log("File Exists.");

            //Converts the bytes into a string
            string data = Encoding.UTF8.GetString(bytes);

            //Decrypts the string
            data = Encryptor.DecryptWithKey(data, Key);

            //Sets the active data to the data from the file
            activeData = new Save_Data(data);
        }
        //Else no save file was found
        else
        {
            LogWarning("File does not exist at " + FileName + " - Loading base save data");

            //Sets the save data to the default value
            ResetData();
        }
    }

    //Method called to reset the save data to its default value
    public void ResetData()
    {
        LogWarning("Resetting data...", true);

        //Creates new save data with default values and sets active data to it
        activeData = new Save_Data("");
    }
}
