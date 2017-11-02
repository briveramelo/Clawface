using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using OET_grid;
using Turing.LevelEditor;

namespace OET_init
{
    public class lib : MonoBehaviour
    {
        public static bool initialized = false;
        public static int Num_x = 0;
        public static int Num_z = 0;
        public static LevelEditorObject tileObject = Resources.Load<LevelEditorObject>("LevelEditorObjects/CommonArea/Floor_Hallway");

        public static void renderGUI(int vpos)
        {
            vpos += OET_lib.ToolLib.header("<b>Init / Save / Load</b>\nSave or Load the Scene with JSON.", vpos, false);

            int width = Screen.width;
            float btWidth = width < 160 ? width - 20 : 160;

            GUI.Label(new Rect(10, vpos, 150, 20), "+x");
            Num_x = EditorGUI.IntSlider(new Rect(90, vpos, width - 100, 20), Num_x, 1, 20);

            GUI.Label(new Rect(10, vpos + 50, 150, 20), "+z");
            Num_z = EditorGUI.IntSlider(new Rect(90, vpos + 50, width - 100, 20), Num_z, 1, 20);

            tileObject = EditorGUI.ObjectField (
                new Rect (width / 2 + btWidth / 2 + 8, vpos + 100, btWidth, 25), 
                tileObject,
                typeof(LevelEditorObject)) as LevelEditorObject;

            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 100, btWidth, 25), "Init") && initialized == false)
            {
                // This will force a database reload
                new LevelObjectDatabase();

                GameObject _platform = new GameObject("LOADED LEVEL");

                _CreateSingleton();

                
                Undo.RegisterCreatedObjectUndo(_platform, "Init the level");

                //GameObject _prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Old/Environment/Hallway/Hallway_Floor.prefab", typeof(GameObject)) as GameObject;

                for (int i = -Num_x; i <= Num_x; i++)
                {
                    for(int j = -Num_z; j <= Num_z; j++)
                    {
                        GameObject _instance = Instantiate(tileObject.Prefab, new Vector3(i * OET_grid.lib.size_x, 0, j * OET_grid.lib.size_z), Quaternion.identity);
                        _instance.name = tileObject.Path;

                        if (_instance.GetComponent<LevelUnit>() == null)
                        {
                            LevelUnit LU = _instance.AddComponent<LevelUnit>() as LevelUnit;
                            LU.defaultState = LevelUnitStates.floor;
                        }
 
                        _instance.transform.SetParent(_platform.transform);
                        Undo.RegisterCreatedObjectUndo(_instance, "Init the level");
                        OET_io.lib.ActiveGameObjects.Add (_instance);
                    }
                }
               
            }

            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 150, btWidth, 25), "Save"))
            {
                string savePath = EditorUtility.SaveFilePanel("Save Level", Application.dataPath, "New Level.level", "level");
                if (savePath == "" || savePath == default(string)) return;
                OET_io.lib.SaveCurrentLevel (savePath);
            }

            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 200, btWidth, 25), "Load"))
            {
                string openPath = EditorUtility.OpenFilePanel ("Open Level", Application.dataPath, "level");
                if (openPath == "" || openPath == default(string)) return;
                OET_io.lib.OpenLevel (openPath);
            }
        }



        static void _CreateSingleton()
        {
            GameObject _prefab;
            GameObject _instance;

            GameObject parent = GameObject.Find ("LOADED LEVEL");

            //_prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player/PlayerSpawner.prefab", typeof(GameObject)) as GameObject;
            LevelEditorObject spawnerData = Resources.Load<LevelEditorObject>("LevelEditorObjects/Special/PlayerSpawn");

            _prefab = spawnerData.Prefab;

            if (_prefab == null)
                Debug.Log("PlayerSpawner PATH error!!!");
            else
            {
                _instance = Instantiate(_prefab, new Vector3(0, 2.5f, 0), Quaternion.identity);
                _instance.name = spawnerData.Path;
                _instance.transform.SetParent (parent.transform);
                OET_io.lib.ActiveGameObjects.Add (_instance);
                Undo.RegisterCreatedObjectUndo(_instance, "Init the level");
            };

            _prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/General/ServiceWrangler.prefab", typeof(GameObject)) as GameObject;

            if(_prefab == null)
            {
                Debug.Log("ServiceWrangler PATH error!!!");
            }
            else
            {
                _instance = Instantiate(_prefab, new Vector3(0, 0, 0), Quaternion.identity);
                Undo.RegisterCreatedObjectUndo(_instance, "Init the level");
            }
        }

    }
}