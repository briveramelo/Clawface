using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OET_init;

namespace OET_grid
{
    public class lib : MonoBehaviour
    {
        public static float size_x = 5.0f;
        public static float size_y = 5.0f;
        public static float size_z = 5.0f;

        static GameObject _prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Old/Environment/Hallway/Hallway_Floor.prefab", typeof(GameObject)) as GameObject;

        static public void sceneGUI()
        {
            _prefab.transform.localPosition = new Vector3(0, 0, 0);

            for (int i = -OET_init.lib.Num_x; i <= OET_init.lib.Num_x; i++)
            {
                for(int j = -OET_init.lib.Num_z; j <= OET_init.lib.Num_z; j++)
                {
                    Vector3 new_position = new Vector3(size_x * i, 0, size_z * j);

                    OET_lib.ToolLib.draft(_prefab, new_position, Color.yellow);
                }
            }
        }
    }
}