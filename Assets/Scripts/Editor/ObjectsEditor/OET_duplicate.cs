using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using OET_lib;

namespace OET_duplicate
{
	public class lib : MonoBehaviour
    {
		public static GameObject[] sceneSelection;
		public static Vector2 scrollPosition = Vector2.zero;

		public static Vector3 handle_v3;
		public static GameObject sceneActiveSelection;

        public static bool _x = true;
        public static bool _z = true;

        public static Vector3[] clones;

		public static void sceneGUI ()
        {
			if (sceneActiveSelection != null && sceneSelection.Length == 1)
            {
				handle_v3 = Handles.PositionHandle(handle_v3, Quaternion.identity);

                handle_v3.y = sceneActiveSelection.transform.position.y;
                handle_v3.z = sceneActiveSelection.transform.position.z;
      
				Vector3 d = handle_v3 - sceneActiveSelection.transform.position;

				int count = (int)(d.x / 5.0f);
                clones = new Vector3[Mathf.Abs((int)count)];

                if (count > 0)
                {
                    for (int i = 1; i < count; i++)
                    {
                        Vector3 tmp = new Vector3(5 * i, 0, 0);
                        clones[i] = tmp;
                        OET_lib.ToolLib.draft(sceneActiveSelection, tmp, Color.yellow);
                    }
                }
                else
                {
                    for (int i = count; i < 0; i++)
                    {
                        Vector3 tmp = new Vector3(5 * i, 0, 0);
                        clones[Mathf.Abs(i) - 1] = tmp;
                        OET_lib.ToolLib.draft(sceneActiveSelection, tmp, Color.yellow);
                    }
                }

				Handles.color = Color.green;
				Handles.DrawDottedLine (handle_v3, sceneActiveSelection.transform.position, 5);
			}
		}

		public static void renderGUI(int vpos, GameObject[] get_sceneSelection, GameObject get_sceneActiveSelection)
		{
			if(get_sceneActiveSelection && sceneActiveSelection != get_sceneActiveSelection) handle_v3 = get_sceneActiveSelection.transform.position + new Vector3(10,0,0);
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

            GUI.Label(new Rect(10, vpos, 150, 20), "x");
            _x = GUI.Toggle(new Rect(160, vpos, 50, 20), _x, "");
            vpos += 20;

            GUI.Label(new Rect(10, vpos, 150, 20), "z");
            _z = GUI.Toggle(new Rect(160, vpos, 50, 20), _z, "");
            vpos += 20;

            _UI();
        }


        static void _UI()
        {
            if(_x)
            {
                _z = false;
            }
            else if(_z)
            {
                _x = false;
            }
        }
	}
}

