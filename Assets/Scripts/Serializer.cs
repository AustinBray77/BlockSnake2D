using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

//Class to contorl the serilization and saving of the gane data
static class Serializer
{
    //Variable to store the game's data
    public static Save_Data activeData = null;

    //Property to calculate the data path to save to
    static string fileName => 
        Application.persistentDataPath + "/blcksnk.bin";

    //Method called to save the data to the hardware
    public static void SaveData()
    {
        //Debug.Log(fileName);

        //Creates a new object to format the data into binary
        BinaryFormatter bf = new BinaryFormatter();

        //Creates file at persisten data path
        FileStream file = File.Create(fileName);
        
        //Serializes the data into the file
        bf.Serialize(file, activeData);

        //Closes the file stream
        file.Close();
    }

    //Method called to load the data from the hardware
    public static void LoadData()
    {
        //Triggers if the file exists at the persistent data path
        if (File.Exists(fileName))
        {
            //Debug.Log("Save loaded");

            //Creates a new object to format the binary data into game data
            BinaryFormatter bf = new BinaryFormatter();

            //Opens the file
            FileStream file = File.Open(fileName, FileMode.Open);

            //Gets the data from the file and deserializes it
            Save_Data data = (Save_Data)bf.Deserialize(file);

            //Closes the file stream
            file.Close();

            //Sets the active data to the data from the file
            activeData = data;
        }
        //Else no save file was found
        else
        {
            //Debug.LogWarning("File does not exist - Loading base save data");

            //Sets the save data to the default value
            ResetData();
        }
    }

    //Method called to reset the save data to its default value
    public static void ResetData()
    {
        //Array to set the first (base) skin to being purchased
        bool[] purchased = new bool[9];
        purchased[0] = true;

        //Creates new save data with default values and sets active data to it
        activeData = new Save_Data(0, purchased, new Level(0), 0, 0, new Settings_Data(QualityController.QualityLevel.Fast, false, true));
    }
}
