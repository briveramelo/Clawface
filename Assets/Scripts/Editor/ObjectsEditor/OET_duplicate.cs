using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using OET_lib;

namespace OET_duplicate
{
	public class lib : MonoBehaviour
    {
        public static lib _instance;

        public static GameObject[] sceneSelection;
		public static Vector2 scrollPosition = Vector2.zero;

		public static Vector3 handle_v3;
		public static GameObject sceneActiveSelection;

        public static bool _x_selected = true;
        public static bool _y_selected = true;
        public static bool _z_selected = true;

        public static bool needUpdateHandle = false;

        public static Vector3[] clones;

        public static void sceneGUI ()
        {
			if (sceneActiveSelection != null && sceneSelection.Length == 1)
            {
				handle_v3 = Handles.PositionHandle(handle_v3, Quaternion.identity);

                DrawBox();

                Handles.color = Color.green;
				Handles.DrawDottedLine (handle_v3, sceneActiveSelection.transform.position, 5);
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
				if(sceneSelection.Length != 1)
                {
                    OET_lib.ToolLib.alertBox ("Duplicate", "Select one single object in the hierarchy to enable this tool. Multiple objects are currently selected.");
				}
                else
                {
					vpos += OET_lib.ToolLib.header ("<b>Duplicate</b>\nDuplicate the selected object in the scene.", vpos, false);

					float btWidth = width < 160 ? width - 20 : 160;
					if (GUI.Button (new Rect (width / 2 - btWidth / 2, vpos, btWidth, 25), "Duplicate"))
                    {

						GameObject newClone = null;
						GameObject p = PrefabUtility.GetPrefabParent(sceneActiveSelection) as GameObject;
						var newSelection = new List<GameObject> ();

						foreach(Vector3 thisClone in clones)
                        {
							if(p != null)
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
							Undo.RegisterCreatedObjectUndo (newClone, "Duplicate");
							newSelection.Add (newClone);
						}
						Selection.objects = newSelection.ToArray ();
					}
				}
			}
            else
            {
                OET_lib.ToolLib.alertBox ("Duplicate", "Select an object in the hierarchy to enable this tool.");
			}


            if (GUI.Button(new Rect(15, vpos, 100, 25), "X-axis"))
            {
                _x_selected = true;
                _y_selected = false;
                _z_selected = false;
                UpdateHandle();
            }

            vpos += 40;

            if (GUI.Button(new Rect(15, vpos, 100, 25), "Y-axis"))
            {
                _x_selected = false;
                _y_selected = true;
                _z_selected = false;
                UpdateHandle();
            }

            vpos += 40;

            if (GUI.Button(new Rect(15, vpos, 100, 25), "Z-axis"))
            {
                _x_selected = false;
                _y_selected = false;
                _z_selected = true;
                UpdateHandle();
            }

            vpos += 40;
        }
            
        static void DrawBox()
        {
            if(_x_selected)
            {
                handle_v3.y = sceneActiveSelection.transform.position.y;
                handle_v3.z = sceneActiveSelection.transform.position.z;
            }

            if (_y_selected)
            {
                handle_v3.x = sceneActiveSelection.transform.position.x;
                handle_v3.z = sceneActiveSelection.transform.position.z;
            }

            if (_z_selected)
            {
                handle_v3.x = sceneActiveSelection.transform.position.x;
                handle_v3.y = sceneActiveSelection.transform.position.y;
            }


            Vector3 d = handle_v3 - sceneActiveSelection.transform.position;

            float distance = _x_selected ? d.x : _y_selected ? d.y : d.z;

            int count = Mathf.Abs((int)(distance / 5.0f));

            clones = new Vector3[count];

            for (int i = 1; i <= count; i++)
            {
                Vector3 new_position;

                if(distance > 0)
                {
                    new_position = _x_selected ? new Vector3(5 * i, 0, 0) : _y_selected ? new Vector3(0, 5 * i, 0) : new Vector3(0, 0, 5 * i);
                }
                else
                {
                    new_position = _x_selected ? new Vector3(-5 * i, 0, 0) : _y_selected ? new Vector3(0, -5 * i, 0) : new Vector3(0, 0, -5 * i);
                }

                 clones[i - 1] = new_position;
                OET_lib.ToolLib.draft(sceneActiveSelection, new_position, Color.yellow);
            }
        }

        static void UpdateHandle()
        {
            Vector3 d = _x_selected ? new Vector3(10, 0, 0) : _y_selected ? new Vector3(0, 10, 0) : new Vector3(0, 0, 10);
            handle_v3 = sceneActiveSelection.transform.position + d;
        }

	}
}

