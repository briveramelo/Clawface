using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AI;
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

            GUI.enabled = !OET_io.lib.LevelIsLoaded;


            // Init button
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

                        //Edge + Wall
                        if(i == -Num_x || i == Num_x || j == -Num_z || j == Num_z)
                        {
                            _AddNavMeshModifier(_instance, OET_lib.NavMeshAreas.NotWalkable);

                            GameObject _wall = Instantiate(tileObject.Prefab, new Vector3(i * OET_grid.lib.size_x, OET_grid.lib.size_y, j * OET_grid.lib.size_z), Quaternion.identity);
                            _wall.transform.SetParent(_platform.transform);
                            Undo.RegisterCreatedObjectUndo(_wall, "Init the level");
                            OET_io.lib.ActiveGameObjects.Add(_wall);
                            _AddNavMeshModifier(_wall, OET_lib.NavMeshAreas.NotWalkable);

                        }
                        else if (_instance.GetComponent<LevelUnit>() == null)
                        {
                            LevelUnit LU = _instance.AddComponent<LevelUnit>() as LevelUnit;
                            LU.defaultState = LevelUnitStates.floor;
                            _AddNavMeshModifier(_instance, OET_lib.NavMeshAreas.Walkable);
                        }
 
                        _instance.transform.SetParent(_platform.transform);
                        Undo.RegisterCreatedObjectUndo(_instance, "Init the level");
                        OET_io.lib.ActiveGameObjects.Add (_instance);
                    }
                }


                GameObject NavSurface = GameObject.Find("NavMeshSurface");

                if(NavSurface)
                    NavSurface.GetComponent<NavMeshSurface>().BuildNavMesh();


                OET_io.lib.SetDirty (true);
            }

            tileObject = EditorGUI.ObjectField (
                new Rect (width / 2 + btWidth / 2 + 8, vpos + 100, btWidth, 25), 
                tileObject,
                typeof(LevelEditorObject)) as LevelEditorObject;

            GUI.enabled = OET_io.lib.LevelIsLoaded;

            // Save button
            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 150, btWidth, 25), "Save"))
            {
                OET_io.lib.SaveCurrentLevel();
            }

            // Save As button
            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 200, btWidth, 25), "Save As"))
            {
                string savePath = EditorUtility.SaveFilePanel("Save Level", Application.dataPath, "New Level.level", "level");
                if (savePath == "" || savePath == default(string)) return;
                OET_io.lib.SaveCurrentLevel (savePath);
            }

            // Close button
            if (GUI.Button (new Rect (width / 2 - btWidth / 2, vpos + 250, btWidth, 25), "Close"))
            {
                if (OET_io.lib.IsDirty)
                {
                    switch (EditorUtility.DisplayDialogComplex(
                        "Save Level", "Do you want to save your level?", 
                        "Save", "Don't Save", "Cancel"))
                    {
                        case 0: // Save
                            string savePath = EditorUtility.SaveFilePanel("Save Level", Application.dataPath, "New Level.level", "level");
                            if (savePath == "" || savePath == default(string)) return;
                            OET_io.lib.SaveCurrentLevel (savePath);
                            OET_io.lib.CloseLevel ();
                            break;

                        case 1: // Don't Save
                            OET_io.lib.CloseLevel ();
                            break;

                        case 2: // Cancel
                            break;
                    }
                }

                OET_io.lib.CloseLevel();
            }

            GUI.enabled = true;

            // Load button
            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 300, btWidth, 25), "Load"))
            {
                if (OET_io.lib.LevelIsLoaded && OET_io.lib.IsDirty)
                {
                    switch (EditorUtility.DisplayDialogComplex(
                            "Save Level", "Do you want to save your level?", 
                            "Save", "Don't Save", "Cancel"))
                        {
                            case 0: // Save
                                string savePath = EditorUtility.SaveFilePanel("Save Level", Application.dataPath, "New Level.level", "level");
                                if (savePath == "" || savePath == default(string)) return;
                                OET_io.lib.SaveCurrentLevel (savePath);
                                OET_io.lib.CloseLevel ();
                                break;

                            case 1: // Don't Save
                                OET_io.lib.CloseLevel ();
                                break;

                            case 2: // Cancel
                                break;
                        }
                }

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


            _CreateSingleton("Music Intensity Manager", parent, "Assets/Prefabs/General/Music Intensity Manager.prefab");
            _CreateSingleton("ServiceWrangler", parent, "Assets/Prefabs/General/ServiceWrangler.prefab");
            _CreateSingleton("SpawnManager", parent, "Assets/Prefabs/Enemies/SpawnManager.prefab");
            _CreateSingleton("NavMeshSurface", parent, "Assets/Prefabs/LevelEditor/Floor Tiles/NavMeshSurface.prefab");
        }

        static void _CreateSingleton(string name, GameObject parent, string PATH)
        {
            if (GameObject.Find(name) == null)
            {
                GameObject _prefab = AssetDatabase.LoadAssetAtPath(PATH, typeof(GameObject)) as GameObject;

                if (_prefab == null)
                {
                    Debug.Log(name + " PATH error");
                }
                else
                {
                    GameObject _instance = Instantiate(_prefab, new Vector3(0, 0, 0), Quaternion.identity);
                    _instance.name = name;
                    _instance.transform.SetParent(parent.transform);
                    Undo.RegisterCreatedObjectUndo(_instance, "Init the level");
                }
            }
        }


        static void _AddNavMeshModifier(GameObject _object, int _state)
        {
            NavMeshModifier _mod = _object.AddComponent<NavMeshModifier>() as NavMeshModifier;
            _mod.overrideArea = true;
            _mod.area = _state;
        }

    }
}