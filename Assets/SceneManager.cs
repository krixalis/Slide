/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public string SceneName;
    void OnLevelWasLoaded(int level)
    {
        string[] fullPath = EditorApplication.currentScene.Split(char.Parse("/"));
        SceneName = fullPath[fullPath.Length - 1];
        SceneName = SceneName.Substring(0, SceneName.IndexOf('.'));
        Debug.Log(SceneName);

        try
        {
            Stream curSceneFile = File.Open(SceneName + ".sceneState", FileMode.Open);
        }
        catch (Exception)
        {
            throw new Exception(String.Format("File {0}.sceneState not found", SceneName));
        }
    }

    void FixedUpdate()
    {
        //Create sceneState
        if (Input.GetButtonDown("DebugSave"))
        {
            //pushed P to save
            if (SceneName == null) return;
            try
            {
                Stream sceneFile = File.Open(SceneName + ".sceneState", FileMode.Open);
                Debug.Log(String.Format("Existing file called \"{0}.sceneState\" found.", SceneName));
            }
            catch
            {
                Debug.Log(String.Format("Could not find file \"{0}.sceneState\", creating...", SceneName));
                //Debug.Log(String.Format("Creating file\"{0}.sceneState\"."));
                //Stream sceneFile = File.Open(SceneName + ".sceneState", FileMode.Create);
                //Debug.Log(String.Format("File \"{0}.sceneState\" created."));

                var powerUps = GameObject.FindGameObjectsWithTag("PowerUp");

                var powerUpStates = powerUps.ToDictionary(powerUp => powerUp, powerUp => powerUp.activeSelf);

                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                var data = new SaveData()
                {
                    PowerUpSaves = powerUpStates
                };

                using (Stream sceneFile = File.Open(SceneName + ".sceneState", FileMode.Create))
                {
                    //serialize directly into that stream.
                    formatter.Serialize(sceneFile, data);
                }
            }
        }
    }

    //thi is our save data structure.
    [Serializable] //needs to be marked as serializable
    struct SaveData
    {
        public Dictionary<GameObject, bool> PowerUpSaves;
    }



    /* from http://forum.unity3d.com/threads/how-to-write-a-file.8864/#post-1345425
    static void test()
    {
        //this is the formatter, you can also use an System.Xml.Serialization.XmlSerializer;
        var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        //initialize some data to save
        var data = new SaveData()
        {
            somefloatvalue = 10.8F,
            someintvalue = 1,
            somestringvalue = "some data"
        };

        //open a filestream to save on
        //notice there is no need to close or flush the stream as it will do it before disposing at the end of the using block.
        using (Stream filestream = File.Open("filename.dat", FileMode.Create))
        {
            //serialize directly into that stream.
            formatter.Serialize(filestream, data);
        }

        //and now how to load that data.

        SaveData loaded_data;
        //again we open a filestream but now with fileMode.Open
        using (Stream filestream = File.Open("filename.dat", FileMode.Open))
        {
            //deserialize directly from that stream.
            loaded_data = (SaveData)formatter.Deserialize(filestream);
        }
    }

    //thi is our save data structure.
    [Serializable] //needs to be marked as serializable
    struct SaveData
    {
        public int someintvalue;
        public float somefloatvalue;
        public string somestringvalue;
    }
    */
//}
