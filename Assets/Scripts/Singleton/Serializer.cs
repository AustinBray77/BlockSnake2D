using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using UnityEngine;
using EncryptorLIB;

//Class to contorl the serilization and saving of the gane data
public class Serializer : Singleton<Serializer>
{
    //Key for encryption
    public static string key => SystemInfo.deviceUniqueIdentifier;
    //Variable to store the game's data
    public Save_Data activeData = null;

    //Property to calculate the data path to save to
    public static string fileName =>
        Application.persistentDataPath + "/blcksnk.bin";

    //Method called to save the data to the hardware
    public IEnumerator SaveData()
    {
        //Try to sign in to google play services
        yield return Instance.StartCoroutine(PlayGamesService.Instance.SignIn(false));


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

            data = info.Name + ":" + data + "\n";

            //Creates file at persistent data path
            FileStream file = File.Create(Application.persistentDataPath + x + ".bin");

            //Encrypts the data
            data = Encryptor.EncryptWithKey(data, key);
            totalData += data;

            x++;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(totalData);



        if (File.Exists(fileName)) File.Delete(fileName);
    }

    //Method called to load the data from the hardware
    public void LoadData()
    {
        //Triggers if the file exists at the persistent data path
        if (File.Exists(fileName))
        {
            //Reads all bytes in the file
            byte[] bytes = File.ReadAllBytes(fileName);

            Debug.Log("Save exists at " + fileName);

            //Converts the bytes into a string
            string data = Encoding.UTF8.GetString(bytes);

            //Decrypts the string
            data = Encryptor.DecryptWithKey(data, key);

            //Sets the active data to the data from the file
            activeData = new Save_Data(data);
        }
        //Else no save file was found
        else
        {
            Debug.LogWarning("File does not exist at " + fileName + " - Loading base save data");

            //Sets the save data to the default value
            ResetData();
        }
    }

    //Method called to reset the save data to its default value
    public void ResetData()
    {
        //Creates new save data with default values and sets active data to it
        activeData = new Save_Data("");
    }
}
