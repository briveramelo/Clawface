using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OET_grid
{
    public class lib : MonoBehaviour
    {
        public static float size_x = 5.0f;
        public static float size_y = 5.0f;
        public static float size_z = 5.0f;

        static Vector3 pos = new Vector3(0, 0, 0);

        static public void sceneGUI()
        {
            for (float x = pos.x - 100.0f; x < pos.x + 100.0f; x += size_x)
            {
                Debug.DrawLine(new Vector3(-100.0f, 0.0f, Mathf.Floor(x / size_x) * size_x + size_x / 2), new Vector3(100.0f, 0.0f, Mathf.Floor(x / size_x) * size_x + size_x / 2),  Color.red);
            }

            for (float z = pos.z - 100.0f; z < pos.z + 100.0f; z += size_z)
            {
                Debug.DrawLine(new Vector3(Mathf.Floor(z / size_z) * size_z + size_z / 2, 0.0f, -100.0f), new Vector3(Mathf.Floor(z / size_z) * size_z + size_z / 2, 0.0f, 100.0f), Color.red);
            }
        }
    }
}