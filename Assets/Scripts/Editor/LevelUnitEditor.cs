using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelUnit))]
public class LevelUnitEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LevelUnit levelUnit = (LevelUnit)target;
        if (GUILayout.Button("cover"))
        {
            levelUnit.TransitionToCoverState();
        }
        if (GUILayout.Button("floor"))
        {

            levelUnit.TransitionToFloorState();
        }
        if (GUILayout.Button("pit"))
        {
            levelUnit.TransitionToPitState();
        }
    }

}
