using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using OET_lib;
using OET_grid;

namespace OET_duplicate
{
	public class lib : MonoBehaviour
    {
        public static GameObject[]  sceneSelection;
        public static GameObject    sceneActiveSelection;

        public static Vector3[] clones;
        public static Vector3   handle_v3;

        public static void sceneGUI ()
        {
            handle_v3 = Handles.PositionHandle(handle_v3, Quaternion.identity);

            if (sceneActiveSelection != null && sceneSelection.Length == 1)
            {
                DrawBox();
                Handles.color = Color.green;
                Handles.DrawDottedLine(handle_v3, sceneActiveSelection.transform.position, 5);
            }
        }

		public static void renderGUI(int vpos, GameObject[] get_sceneSelection, GameObject get_sceneActiveSelection)
		{

            if (get_sceneActiveSelection && sceneActiveSelection != get_sceneActiveSelection)
            {
                handle_v3 = get_sceneActiveSelection.transform.position + new Vector3(10, 0, 0);
            }

			sceneSelection = get_sceneSelection;

			int width = Screen.width;
			sceneActiveSelection = get_sceneActiveSelection;

			if (get_sceneActiveSelection != null)
            {
                if (sceneSelection.Length != 1)
                {
                    OET_lib.ToolLib.alertBox("Duplicate", "Select one single object in the hierarchy to enable this tool. Multiple objects are currently selected.");
                }
                else
                {
                    vpos += OET_lib.ToolLib.header("<b>Duplicate</b>\nDuplicate the selected object in the scene.", vpos, false);

                    float btWidth = width < 160 ? width - 20 : 160;

                    if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos, btWidth, 25), "Duplicate"))
                    {
                        var newSelection = new List<GameObject>();

                        GameObject newClone = null;
                        GameObject p = PrefabUtility.GetPrefabParent(sceneActiveSelection) as GameObject;

                        foreach (Vector3 thisClone in clones)
                        {
                            if (p != null)
                            {
                                newClone = PrefabUtility.InstantiatePrefab(p) as GameObject;
                                newClone.transform.position = thisClone + sceneActiveSelection.transform.position;
                                newClone.transform.rotation = sceneActiveSelection.transform.rotation;
                            }
                            else
                            {
                                newClone = Instantiate(sceneActiveSelection.gameObject, thisClone + sceneActiveSelection.transform.position, sceneActiveSelection.transform.rotation) as GameObject;
                                newClone.transform.localScale = sceneActiveSelection.transform.localScale;
                            }
                            newClone.transform.parent = sceneActiveSelection.transform.parent;
                            newClone.transform.localScale = sceneActiveSelection.transform.localScale;
                            Undo.RegisterCreatedObjectUndo(newClone, "Duplicate");
                            newSelection.Add(newClone);
                        }

                        Selection.objects = newSelection.ToArray();
                    }
                }
            }
            else
            {
                OET_lib.ToolLib.alertBox ("Duplicate", "Select an object in the hierarchy to enable this tool.");
			}
        }

        static void DrawBox()
        {
            Vector3 distance = handle_v3 - sceneActiveSelection.transform.position;

            List<Vector3> positions = new List<Vector3>();

            float dx = distance.x;
            float dy = distance.y;
            float dz = distance.z;

            int count_x = Mathf.Abs((int)(dx / OET_grid.lib.size_x));
            int count_y = Mathf.Abs((int)(dy / OET_grid.lib.size_y));
            int count_z = Mathf.Abs((int)(dz / OET_grid.lib.size_z));

            float factor_x = dx > 0 ? OET_grid.lib.size_x : -OET_grid.lib.size_x;
            float factor_y = dy > 0 ? OET_grid.lib.size_y : -OET_grid.lib.size_y;
            float factor_z = dz > 0 ? OET_grid.lib.size_z : -OET_grid.lib.size_z;


            for (int i = 0; i <= count_x; i++)
            {
                for (int j = 0; j <= count_y; j++)
                {
                    for (int k = 0; k <= count_z; k++)
                    {
                        if (i == 0 && j == 0 && k == 0) continue;

                        Vector3 new_position;

                        new_position = new Vector3(factor_x * i, factor_y * j, factor_z * k);

                        positions.Add(new_position);

                        OET_lib.ToolLib.draft(sceneActiveSelection, new_position, Color.yellow);
                    }
                }
            }

            clones = positions.ToArray();   
        }
	}
}

