using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OET_save
{
    public class lib : MonoBehaviour
    {
        public static void renderGUI(int vpos)
        {

            vpos += OET_lib.ToolLib.header("<b>Sace / Load</b>\nSave or Load the Scene with JSON.", vpos, false);

            int width = Screen.width;
            float btWidth = width < 160 ? width - 20 : 160;

            
            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos, btWidth, 25), "Save"))
            {
                Debug.Log("Put you saving code in here, my baby.");           
            }

            if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos + 50, btWidth, 25), "Load"))
            {
                Debug.Log("Put you loading code in here, my baby.");
            }
        }
    }
}