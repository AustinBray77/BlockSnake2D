using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using EncryptorLIB;

//Class to contorl the serilization and saving of the gane data
static class Serializer
{
    //Key for encryption
    public static string key => SystemInfo.deviceUniqueIdentifier;
    //Variable to store the game's data
    public static Save_Data activeData = null;

    //Property to calculate the data path to save to
    public static string fileName =>
        Application.persistentDataPath + "/blcksnk.bin";

    //Method called to save the data to the hardware
    public static void SaveData()
    {
        //Creates file at persistent data path
        FileStream file = File.Create(fileName);

        //Encrypts the data
        string data = Encryptor.EncryptWithKey(activeData.ToString(), key);

        //Writes it to the file
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        file.Write(bytes, 0, bytes.Length);

        //Closes the file stream
        file.Close();
    }

    //Method called to load the data from the hardware
    public static void LoadData()
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
    public static void ResetData()
    {
        //Creates new save data with default values and sets active data to it
        activeData = new Save_Data("");
    }
}
