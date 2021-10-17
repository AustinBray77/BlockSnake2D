using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

static class Serializer
{
    public static Save_Data activeData = null;

    static string fileName => 
        Application.persistentDataPath + "/blcksnk.bin";

    public static void SaveData()
    {
        Debug.Log(fileName);

        BinaryFormatter bf = new BinaryFormatter();

        //Creates file at persisten data path
        FileStream file = File.Create(fileName);
        
        //Serializes the data into the file
        bf.Serialize(file, activeData);

        //Closes the file stream
        file.Close();
    }

    public static void LoadData()
    {
        if (File.Exists(fileName))
        {
            Debug.Log("Save loaded");

            BinaryFormatter bf = new BinaryFormatter();

            //Opens the file
            FileStream file = File.Open(fileName, FileMode.Open);

            //Gets the data from the file
            Save_Data data = (Save_Data)bf.Deserialize(file);

            //Closes the file stream
            file.Close();

            activeData = data;
        }
        else
        {
            Debug.LogWarning("File does not exist - Loading base save data");

            bool[] purchased = new bool[3];
            purchased[0] = true;

            activeData = new Save_Data(0, purchased, new Level(0), 0, 0);
        }
    }
}
