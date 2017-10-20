using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using OET_grid;


namespace OET_init
{
    public class lib : MonoBehaviour
    {
        public static bool initialized = false;
        public static int Num_x = 0;
        public static int Num_z = 0;

        public static void renderGUI(int vpos)
        {

            vpos += OET_lib.ToolLib.header("<b>Sace / Load</b>\nSave or Load the Scene with JSON.", vpos, false);

            int width = Screen.width;
            float btWidth = width < 160 ? width - 20 : 160;

            GUI.Label(new Rect(10, vpos, 150, 20), "+x");
            Num_x = EditorGUI.IntSlider(new Rect(90, vpos, width - 100, 20), Num_x, 1, 20);

            GUI.Label(new Rect(10, vpos + 50, 150, 20), "+z");
            Num_z = EditorGUI.IntSlider(new Rect(90, vpos + 50, width - 100, 20), Num_z, 1, 20);

            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 100, btWidth, 25), "Init") && initialized == false)
            {
                GameObject _platform = new GameObject("Platform");

                GameObject _prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Old/Environment/Hallway/Hallway_Floor.prefab", typeof(GameObject)) as GameObject;

                for (int i = -Num_x; i <= Num_x; i++)
                {
                    for(int j = -Num_z; j <= Num_z; j++)
                    {
                        GameObject _instance = Instantiate(_prefab, new Vector3(i * OET_grid.lib.size_x, 0, j * OET_grid.lib.size_z), Quaternion.identity);
         
                        _instance.transform.SetParent(_platform.transform);
                    }
                }
               
            }

            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 150, btWidth, 25), "Save"))
            {
                Debug.Log("Put you saving code in here, my baby.");
            }

            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 200, btWidth, 25), "Load"))
            {
                Debug.Log("Put you loading code in here, my baby.");
            }
        }
    }
}