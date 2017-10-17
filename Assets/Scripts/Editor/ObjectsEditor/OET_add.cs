using UnityEngine;
using UnityEditor;
using System.Collections;
using OET_lib;

namespace OET_add
{
	public class lib : MonoBehaviour
    {
		public static bool clickToAddEnabled = false;
        public static bool previewDraw = false;

        public static GameObject projectActiveSelection;
		public static Vector3 mousePositionInScene;
	
		public static void sceneGUI ()
        {
			if (clickToAddEnabled && projectActiveSelection != null)
            {
				if(previewDraw)
                {
                    OET_lib.ToolLib.draft (projectActiveSelection, mousePositionInScene - projectActiveSelection.transform.position, Color.green);
				}
			}
		}

		public static void renderGUI(int vpos, GameObject get_projectActiveSelection)
		{
			projectActiveSelection = get_projectActiveSelection;
			int width = Screen.width;
			int height = Screen.height;
			int btWidth = width < 160 ? width - 20 : 160;

			GUIStyle styleInfoText = new GUIStyle(GUI.skin.box);
			styleInfoText.wordWrap = true;
			styleInfoText.normal.textColor = GUI.skin.label.normal.textColor;
			styleInfoText.alignment = TextAnchor.MiddleLeft;

			if (projectActiveSelection == null)
            {
                OET_lib.ToolLib.alertBox ("Prefab Placement", "Select a prefab in the project window to enable this tool.");
			}
            else
            {
				if (projectActiveSelection != null)
                {
					if (clickToAddEnabled)
                    {
						vpos += OET_lib.ToolLib.header ("<b>Click to Add</b>\nClick anywhere in the scene to add " + projectActiveSelection.name, vpos, true);
					}
                    else
                    {
						vpos += OET_lib.ToolLib.header ("<b>Click to Add</b>\nClick on the button below then click on the scene to add "  + projectActiveSelection.name, vpos, true);
					}
				}

				Texture2D projectPreview = AssetPreview.GetAssetPreview (projectActiveSelection);
				if(height > 310)
                {
					Color saveBg = GUI.backgroundColor;

					if(clickToAddEnabled)
                    {
						GUI.backgroundColor = new Color(.5f, 0f, 0f, 1);
					}

					clickToAddEnabled = GUI.Toggle (new Rect (width / 2 - btWidth / 2, vpos, btWidth, 40), clickToAddEnabled, "Add to Scene", "button");
					if(clickToAddEnabled) GUI.backgroundColor = saveBg;
					vpos += 50;

					if (projectPreview != null)
                    {
						GUI.Box (new Rect (width / 2 - 64, vpos, 128, 128), projectPreview);
					}
				}
                else
                {
					int bth = height - vpos - 30;
					if(bth > 128) bth = 128;

					clickToAddEnabled = GUI.Toggle (new Rect ((width - bth - 30)/2 - btWidth / 2, vpos, btWidth, bth > 40 ? 40 : bth), clickToAddEnabled, "Add to Scene", "Button");
					if (projectPreview != null)
                    {
						GUI.Box (new Rect (width -  bth - 10, vpos, bth, bth), projectPreview);
					}
				}
			}
		}

		public static bool editorMouseEvent(Event e, GameObject projectActiveSelection)
        {
			previewDraw = false;
			if (clickToAddEnabled && projectActiveSelection != null)
            {
				if(EditorWindow.mouseOverWindow != null && EditorWindow.mouseOverWindow.ToString () == " (UnityEditor.SceneView)")
                {
					Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, 1000.0f)) 
					{
                        mousePositionInScene = hit.point;

 //                       ConvertToGrid(mousePositionInScene);
                  
                        previewDraw = true;
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
						{
							GameObject newAsset = Instantiate((GameObject)projectActiveSelection, ConvertToGrid(mousePositionInScene), Quaternion.identity) as GameObject;
							newAsset.name = projectActiveSelection.name;
							newAsset.transform.rotation = projectActiveSelection.transform.rotation;
							Undo.RegisterCreatedObjectUndo(newAsset, "Add object to scene");
							Selection.activeObject = newAsset;
						}
					}
				}
			}
			return clickToAddEnabled;
		}


        public static Vector3 ConvertToGrid(Vector3 mousePositionInScene)
        {
            float width  = 5.0f;
            float height = 5.0f;

            float Grid_x = Mathf.Floor(mousePositionInScene.x / width) * width + width / 2;

            float Grid_z = Mathf.Floor(mousePositionInScene.z / height) * height + height / 2;

            return new Vector3(Grid_x, mousePositionInScene.y, Grid_z);
        }
	}
}

