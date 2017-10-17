﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using OET_lib;
using OET_add;
using OET_duplicate;
using OET_grid;

public class ObjectsTools : EditorWindow
{
	int activeToolbar = 0;
	int lastActiveToolbar = 0;

    string[] toolbarStrings = new string[] {"Add", "Duplicate", "Replace"};

    GameObject projectActiveSelection;
	GameObject sceneActiveSelection;
	GameObject[] projectSelection;
	GameObject[] sceneSelection;
			
	[MenuItem ("Window/Objects Tools")]

	public static void init ()
	{
		ObjectsTools window = (ObjectsTools)EditorWindow.GetWindow(typeof(ObjectsTools));
		window.titleContent = new GUIContent("Objects Tools");
		window.Show();
		window.minSize = new Vector2 (600f, 120f);
	}

	void OnInspectorUpdate()
    {
        Repaint();
    }

	void OnEnable()
    {
        SceneView.onSceneGUIDelegate += SceneGUI;
    }

	void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= SceneGUI;
    }

	// Detect mouse events in the scene
	private void SceneGUI( SceneView sceneview )
	{
        //bool cancelEvent = false;
        Event e = Event.current;

        // Add
        if (activeToolbar == 0)
        { 
			if(OET_add.lib.editorMouseEvent (e, projectActiveSelection))
            {
				//cancelEvent = true;
				HandleUtility.AddDefaultControl (GUIUtility.GetControlID (GetHashCode (), FocusType.Passive));
			}
            OET_add.lib.sceneGUI ();
            OET_grid.lib.sceneGUI();
		}

        // Clone
        if (activeToolbar == 1)
        { 
            OET_duplicate.lib.sceneGUI ();
		}
        

		HandleUtility.Repaint ();

		if (activeToolbar != lastActiveToolbar)
        {
			HandleUtility.Repaint ();
			lastActiveToolbar = activeToolbar;
		}
	}	
			

	void OnGUI ()
	{
		int width = Screen.width;
		int vpos = 10;

		if (Selection.activeGameObject)
        {
			if (AssetDatabase.Contains (Selection.activeGameObject))
            {
				projectSelection = Selection.gameObjects;
				projectActiveSelection = Selection.activeGameObject;
			}
            else
            {
				sceneSelection = Selection.gameObjects;
				sceneActiveSelection = Selection.activeGameObject;
			}
        }
        else
        {
			sceneSelection = null;
			sceneActiveSelection = null;
		}

		GUIStyle styleInfoText = new GUIStyle(GUI.skin.label);
		styleInfoText.wordWrap = true;

		float barWidth = 7 * 35;
		if(barWidth > width - 15) barWidth = width - 15;
		activeToolbar = GUI.Toolbar(new Rect(width / 2 - barWidth / 2, vpos, barWidth, 24), activeToolbar, toolbarStrings);
		vpos += 40;

        if (activeToolbar == 0) OET_add.lib.renderGUI(vpos, projectActiveSelection);
        if (activeToolbar == 1) OET_duplicate.lib.renderGUI(vpos, sceneSelection, sceneActiveSelection);
        if (activeToolbar == 2) OET_replace.lib.renderGUI(vpos, sceneSelection, projectActiveSelection);
    }
}