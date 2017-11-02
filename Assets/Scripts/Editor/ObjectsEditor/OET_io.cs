using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.LevelEditor;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OET_io
{
    public class lib
    {
        public static List<GameObject> ActiveGameObjects = new List<GameObject>();

        public static void SaveCurrentLevel (string savePath)
        {
            Debug.Log (string.Format("Saving level to {0}.", savePath));
            SerializableLevel level = SerializableLevel.MakeSerializableLevel (ActiveGameObjects);
            Stream saveStream = File.Open (savePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize (saveStream, level);
            saveStream.Close();
        }

        public static void OpenLevel (string openPath)
        {
            CloseLevel();

            Debug.Log (string.Format("Opening level from {0}.", openPath));
            Stream openStream = File.Open (openPath, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            SerializableLevel deserializedLevel = formatter.Deserialize (openStream) as SerializableLevel;
            ActiveGameObjects = deserializedLevel.ReconstructLevel(new LevelObjectDatabase());
            openStream.Close();
        }

        public static void CloseLevel ()
        {
            GameObject parent = GameObject.Find ("LOADED LEVEL");
            if (parent == null)
            {
                Debug.LogWarning ("Tried to close the level, but no loaded level was found.");
            }

            MonoBehaviour.DestroyImmediate (parent);
        }
    }
}