using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Turing.LevelEditor;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ModMan;

namespace OET_io
{
    public class lib
    {
        public const string LOADED_LEVEL_NAME = "LOADED LEVEL";
        public const string LOADED_LEVEL_DIRTY_DELIMITER = "*";

        public static List<GameObject> ActiveGameObjects = new List<GameObject>();

        public static void SetDirty (bool dirty)
        {
            GameObject levelObject = GameObjectExtensions.FindWithSubstring (LOADED_LEVEL_NAME + LOADED_LEVEL_DIRTY_DELIMITER);
            if (levelObject != null)
            {
                if (!dirty)
                {
                    levelObject.name = string.Format("{0}|{1}", LOADED_LEVEL_NAME, LoadedLevelPath);
                    return;
                }
            }

            else
            {
                levelObject = GameObjectExtensions.FindWithSubstring (LOADED_LEVEL_NAME);
                if (levelObject != null)
                {
                    if (dirty)
                    {
                        levelObject.name = string.Format("{0}{1}|{2}", 
                            LOADED_LEVEL_NAME, LOADED_LEVEL_DIRTY_DELIMITER, LoadedLevelPath);
                        return;
                    }
                } 

                else
                {
                    Debug.LogError ("Failed to find a loaded level object!");
                }
            }
        }

        public static GameObject LoadedLevelObject
        {
            get
            {
                GameObject result = GameObjectExtensions.
                    FindWithSubstring (LOADED_LEVEL_NAME + 
                    LOADED_LEVEL_DIRTY_DELIMITER);

                if (result == null)
                    result = GameObjectExtensions.FindWithSubstring (LOADED_LEVEL_NAME);

                return result;
            }
        }

        public static string LoadedLevelPath
        {
            get
            {
                string[] split = LoadedLevelObject.name.Split('|');
                if (split.Length < 2) return default(string);

                return split[1];
            }
        }

        public static bool IsDirty
        {
            get
            {
                GameObject levelObject = 
                    GameObjectExtensions.FindWithSubstring (LOADED_LEVEL_NAME + 
                    LOADED_LEVEL_DIRTY_DELIMITER);

                return levelObject != null;
            }
        }

        public static bool LevelIsLoaded
        {
            get
            {
                return GameObjectExtensions.FindWithSubstring (LOADED_LEVEL_NAME) != null;
            }
        }

        public static void SaveCurrentLevel ()
        {
            string levelPath = LoadedLevelPath;
            if (levelPath == null || levelPath.Equals(default(string)) || levelPath == "")
            {
                string savePath = EditorUtility.SaveFilePanel("Save Level", Application.dataPath, "New Level.level", "level");
                if (savePath == "" || savePath == default(string)) return;
                SaveCurrentLevel (savePath);
            }

            else
            {
                SaveCurrentLevel (levelPath);
            }
        }

        public static void SaveCurrentLevel (string savePath)
        {
            SerializableLevel level = SerializableLevel.MakeSerializableLevel (ActiveGameObjects);
            Stream saveStream = File.Open (savePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize (saveStream, level);
            saveStream.Close();

            SetDirty (false);

            LoadedLevelObject.name = string.Format("{0}|{1}", LOADED_LEVEL_NAME, savePath);
        }

        public static void OpenLevel (string openPath)
        {
            if (LevelIsLoaded) CloseLevel();
            
            Stream openStream = File.Open (openPath, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            SerializableLevel deserializedLevel = formatter.Deserialize (openStream) as SerializableLevel;
            ActiveGameObjects = deserializedLevel.ReconstructLevel(new LevelObjectDatabase());
            openStream.Close();

            LoadedLevelObject.name = string.Format("{0}|{1}", LOADED_LEVEL_NAME, openPath);
        }

        public static void CloseLevel ()
        {
            if (!LevelIsLoaded)
            {
                Debug.LogWarning ("Tried to close level but no level was found!");
            }

            MonoBehaviour.DestroyImmediate (LoadedLevelObject);
        }
    }
}