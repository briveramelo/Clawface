using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OET_lib;

namespace OET_group
{
    public class lib : MonoBehaviour
    {
        public static string groupName = "New group";
        public static void renderGUI(int vpos, GameObject[] sceneSelection)
        {
            int width = Screen.width;

            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.MiddleCenter;

            if (sceneSelection != null)
            {
                if (sceneSelection.Length != 1)
                {
                    OET_lib.ToolLib.alertBox("Duplicate", "Select one single object in the hierarchy to enable this tool. Multiple objects are currently selected.");
                }
                else
                {
                    vpos += OET_lib.ToolLib.header("<b>Group Objects</b>\nGroup selected objects under a newly created game object.", vpos, true);

                    if (width < 300)
                    {
                        GUI.Label(new Rect(10, vpos, width - 20, 20), "Group name");
                        vpos += 15;
                        groupName = GUI.TextField(new Rect(10, vpos, width - 20, 20), groupName, 25);
                    }
                    else
                    {
                        GUI.Label(new Rect(10, vpos, width / 3, 20), "Group name");
                        groupName = GUI.TextField(new Rect(width / 3 + 10, vpos, width - width / 3 - 20, 20), groupName, 25);
                    }

                    vpos += 30;//height - vpos - 65 > 0 ? 40 : height - vpos - 65;

                    // GROUP
                    int btWidth = width < 160 ? width - 20 : 160;
                    if (GUI.Button(new Rect(width / 2 - btWidth / 2, vpos, btWidth, 25), "Group"))
                    {
                        GameObject goGroup = new GameObject(groupName);
                        goGroup.transform.parent = Selection.activeGameObject.transform.parent;
                        Undo.RegisterCreatedObjectUndo(goGroup, "Group objects");

                        foreach (GameObject _obj in Selection.gameObjects)
                        {
                            // Only direct children to preserve hierarchy
                            if (_obj.transform.parent == goGroup.transform.parent)
                            {
                                Undo.SetTransformParent(_obj.transform, goGroup.transform, "Group objects");
                            }
                        }

                        var theParent = new List<GameObject>(1);
                        theParent.Add(goGroup);
                        Selection.objects = theParent.ToArray();
                    }
                }
            }
            else
            {
                OET_lib.ToolLib.alertBox("Grouping", "Select objects in the hierarchy or in the scene view to enable this tool.");
            }
        }
    }
}

