using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor {

    LevelManager _target;

    public override void OnInspectorGUI() {
        _target = target as LevelManager;

        //EditorGUILayout.LabelField ("Level loaded: " + _target.LevelLoaded.ToString());

        base.OnInspectorGUI();
    }
}
