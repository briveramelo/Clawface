using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using OET_lib;
using Turing.LevelEditor;

namespace OET_replace
{
	public class lib : MonoBehaviour
    {
		public static bool preserveOrientation = true;
		public static bool preserveScale = false;

		//public static void renderGUI(int vpos, GameObject[] sceneSelection, GameObject projectActiveSelection)
        public static void renderGUI(int vpos, GameObject[] sceneSelection, LevelEditorObject projectActiveSelection)
		{
			int width = Screen.width;
			int height = Screen.height;
			bool colMode = height < 350;

			if (sceneSelection != null)
            {
				if(projectActiveSelection != null)
                {
					GUIStyle styleInfoText = new GUIStyle (GUI.skin.box);
					styleInfoText.wordWrap = true;
					styleInfoText.fontSize = 9;
					styleInfoText.normal.textColor = GUI.skin.label.normal.textColor;
					styleInfoText.alignment = TextAnchor.MiddleLeft;

					if(height > 220)
                    {
						vpos += OET_lib.ToolLib.header ("<b>Replacement</b>\nReplace the current selection in the scene with the latest selected prefab in the project window (previewed below).", vpos, false);
					}
					Texture2D projectPreview = UnityEditor.AssetPreview.GetAssetPreview (projectActiveSelection.Prefab);
					if (projectPreview != null)
                    {
						if(colMode)
                        {
							int bxsize = vpos + 128 + 30 > height ? height - vpos - 30 : 128;
							if(width < 330) bxsize = width - 210;
							if(vpos + bxsize + 30 > height) bxsize = height - vpos - 30;
							GUI.Box (new Rect (width - bxsize - 10, vpos, bxsize, bxsize), projectPreview);
						}
					}

					GUI.Label (new Rect (10, vpos, 150, 20), "Preserve Orientation");
					preserveOrientation = GUI.Toggle (new Rect (160, vpos, 50, 20), preserveOrientation, "");
					vpos += 20;

					GUI.Label (new Rect (10, vpos, 150, 20), "Preserve Scale");
					preserveScale = GUI.Toggle (new Rect (160, vpos, 50, 20), preserveScale, "");
					vpos += 35;

					int btWidth = width < 160 ? width - 20 : 160;
					if (GUI.Button (new Rect (colMode ? width / 3 - btWidth / 2 : width / 2 - btWidth / 2, vpos, btWidth, 25), "Replace"))
                    {
                        var newSelection = new List<GameObject>();
                        foreach (GameObject obj in sceneSelection)
                        {
                            GameObject newObject = PrefabUtility.InstantiatePrefab(projectActiveSelection) as GameObject;
                            newObject.transform.parent = obj.transform.parent;
                            newObject.transform.position = obj.transform.position;
                            if (preserveScale) newObject.transform.localScale = obj.transform.localScale;
                            if (preserveOrientation) newObject.transform.eulerAngles = obj.transform.eulerAngles;
                            newSelection.Add(newObject);
                            Undo.RegisterCreatedObjectUndo(newObject, "Objects replacement");
                            Undo.DestroyObjectImmediate(obj);
                        }
                        Selection.objects = newSelection.ToArray();
                    }
					vpos += 30;
					if(!colMode)
                    {
						GUI.Box (new Rect (width / 2 - 64, vpos, 128, 128), projectPreview);
					}
				}
                else
                {
					OET_lib.ToolLib.alertBox ("Objects Replacement", "Select a prefab in the project window to enable this tool.");
				}
			}
            else
            {
                OET_lib.ToolLib.alertBox ("Objects Replacement", "Select objects in the hierarchy or in the scene view to enable this tool.");	
			}
		}
	}
}

