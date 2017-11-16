using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using OET_lib;
using Turing.LevelEditor;

namespace OET_add
{
	public class lib : MonoBehaviour
    {
		public static bool clickToAddEnabled = false;
        public static bool previewDraw = false;

        //public static GameObject projectActiveSelection;
        public static LevelEditorObject projectActiveSelection;
		public static Vector3 mousePositionInScene;

        static Vector2 scrollPos = Vector2.zero;

        static bool usingDB = false;
        static LevelObjectDatabase prefabDatabase;
        static int selectedCategory = 0;
        static List<KeyValuePair<string, LevelEditorObject>> selectedObjects;

        public static void sceneGUI ()
        {
			if (clickToAddEnabled && projectActiveSelection != null)
            {
				if(previewDraw)
                {
                    OET_lib.ToolLib.draft (projectActiveSelection.Prefab, mousePositionInScene - projectActiveSelection.Prefab.transform.position, Color.green);
                }
            }
		}

		//public static void renderGUI(int vpos, GameObject get_projectActiveSelection)
        public static void renderGUI (int vpos, LevelEditorObject get_projectActiveSelection)
		{

            if(!usingDB) projectActiveSelection = get_projectActiveSelection;

			int width = Screen.width;
			int height = Screen.height;
			int btWidth = width < 160 ? width - 20 : 160;

			GUIStyle styleInfoText = new GUIStyle(GUI.skin.box);
			styleInfoText.wordWrap = true;
			styleInfoText.normal.textColor = GUI.skin.label.normal.textColor;
			styleInfoText.alignment = TextAnchor.MiddleLeft;

            usingDB = GUI.Toggle(new Rect(10, vpos + 100, btWidth, 40), usingDB, "");
            GUI.Label(new Rect(30, vpos + 100, btWidth, 40), "Using Prefabs DataBase");

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


                Texture2D projectPreview = UnityEditor.AssetPreview.GetAssetPreview (projectActiveSelection.Prefab);
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


            if (usingDB)
                RenderDB(vpos);
        }

		public static bool editorMouseEvent(Event e)
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
							GameObject newAsset = Instantiate((GameObject)projectActiveSelection.Prefab, ConvertToGrid(mousePositionInScene), Quaternion.identity) as GameObject;
							newAsset.name = projectActiveSelection.Path;
							newAsset.transform.rotation = projectActiveSelection.Prefab.transform.rotation;
							Undo.RegisterCreatedObjectUndo(newAsset, "Add object to scene");
							Selection.activeObject = newAsset;
                            newAsset.transform.SetParent (OET_io.lib.LoadedLevelObject.transform);
                            OET_io.lib.ActiveGameObjects.Add (newAsset);
                            OET_io.lib.SetDirty (true);
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

            float Grid_x = Mathf.Floor((mousePositionInScene.x + width  / 2) / width)  * width;
            float Grid_z = Mathf.Floor((mousePositionInScene.z + height / 2) / height) * height;

            RaycastHit hit;

            if (Physics.Raycast(new Vector3(Grid_x, 1000.0f, Grid_z), Vector3.down, out hit))
            {
                return new Vector3(Grid_x, hit.point.y, Grid_z);
            }

            return new Vector3(Grid_x, mousePositionInScene.y, Grid_z);
        }


        public static void RenderDB(int vpos)
        {
            if (prefabDatabase == null)
                prefabDatabase = new LevelObjectDatabase();

            int width = Screen.width;
            int height = Screen.height;

            vpos += 150;

            Rect toolbarPos = new Rect (width / 4, vpos, width / 2, 2 * EditorGUIUtility.singleLineHeight);
            selectedCategory = GUI.Toolbar (toolbarPos, selectedCategory, prefabDatabase.GetFancyCategories);
            selectedObjects = prefabDatabase.GetObjects(selectedCategory);

            GUI.Label (new Rect (30, vpos, width / 4 - 8, toolbarPos.height), "Categories");

            if (GUI.Button (new Rect(toolbarPos.x + toolbarPos.width + 8, vpos, 128, toolbarPos.height), "Refresh"))
                prefabDatabase.LoadLevelObjects();

            vpos += Mathf.CeilToInt(toolbarPos.height) + (int)EditorGUIUtility.singleLineHeight;

            if (selectedObjects == null) 
                Debug.LogError ("Failed to get selected objects!");

            int Num = selectedObjects.Count;
            int IconSize = 64;
            int IconSpace = 100;
            int count_x = width / 100;
            int count_y = Num % count_x == 0 ? Num / count_x : Num / count_x + 1;

            scrollPos = GUI.BeginScrollView(new Rect(0, vpos, width, height - vpos), scrollPos, new Rect(0, 0, width, count_y * IconSpace));


            int localvpos = 0;
            int localhpos = (IconSpace - IconSize) / 2;

            for(int i = 0; i < Num; i++)
            {
                Texture2D Preview = UnityEditor.AssetPreview.GetAssetPreview(selectedObjects[i].Value.Prefab);
                GUIContent content = new GUIContent (Preview, selectedObjects[i].Value.Name);

                if (Preview != null)
                {
                    if(GUI.Button(new Rect(localhpos, localvpos, IconSize, IconSize), content))
                    {
                        projectActiveSelection = selectedObjects[i].Value;
                    }

                    localhpos += IconSpace;

                    if(localhpos + IconSpace >= width)
                    {
                        localhpos = (IconSpace - IconSize) / 2;
                        localvpos += IconSpace;
                    }
                }
            }

            GUI.EndScrollView();
        }
	}
}

