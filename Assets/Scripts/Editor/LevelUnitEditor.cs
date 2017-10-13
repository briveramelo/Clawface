using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelUnit))]
[CanEditMultipleObjects]
public class LevelUnitEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //Uncomment this code only for debugging

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
