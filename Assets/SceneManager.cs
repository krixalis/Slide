using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    void OnLevelWasLoaded(int level)
    {
        string[] fullPath = EditorApplication.currentScene.Split(char.Parse("/"));
        string path = fullPath[fullPath.Length - 1];
        path = path.Substring(0, path.IndexOf('.'));
        Debug.Log(path);

        try
        {
            Stream curSceneFile = File.Open(path + ".sceneState", FileMode.Open);
        }
        catch (Exception)
        {
            
            throw new Exception(String.Format("File {0}.sceneState not found", path));
        }
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
}
