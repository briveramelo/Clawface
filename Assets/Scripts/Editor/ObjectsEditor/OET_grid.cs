using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OET_grid
{
    public class lib : MonoBehaviour
    {
        public static float width = 5.0f;
        public static float height = 5.0f;

        static Vector3 pos = new Vector3(0, 0, 0);

        static public void sceneGUI()
        {
            for (float x = pos.x - 100.0f; x < pos.x + 100.0f; x += width)
            {
                Debug.DrawLine(new Vector3(-100.0f, 0.0f, Mathf.Floor(x / width) * width), new Vector3(100.0f, 0.0f, Mathf.Floor(x / width) * width),  Color.red);
            }

            for (float z = pos.z - 100.0f; z < pos.z + 100.0f; z += height)
            {
                Debug.DrawLine(new Vector3(Mathf.Floor(z / height) * height, 0.0f, -100.0f), new Vector3(Mathf.Floor(z / height) * height, 0.0f, 100.0f), Color.red);
            }
        }
    }
}