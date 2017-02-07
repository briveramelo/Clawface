using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FireTrap))]
public class FireTrapEditor : Editor {

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        var _target = target as FireTrap;

        EditorGUILayout.Space();

        _target.TrapMode = (FireTrap.Mode)EditorGUILayout.EnumPopup ("Trap Mode", _target.TrapMode);

        switch (_target.TrapMode) {
            case FireTrap.Mode.ContinuousOpenClose:
                _target.OpenTime = EditorGUILayout.FloatField ("Open Time", _target.OpenTime);
                _target.CloseTime = EditorGUILayout.FloatField ("Close Time", _target.CloseTime);
                _target.StayOpenTime = EditorGUILayout.FloatField ("Stay Open Time", _target.StayOpenTime);
                _target.StayClosedTime = EditorGUILayout.FloatField ("Stay Closed Time", _target.StayClosedTime);
                break;

            case FireTrap.Mode.ContinuousStream:
                break;

            case FireTrap.Mode.PressureTrigger:
                _target.OpenTime = EditorGUILayout.FloatField ("Open Time", _target.OpenTime);
                _target.CloseTime = EditorGUILayout.FloatField ("Close Time", _target.CloseTime);
                break;
        }

        _target.DamagePerSecond = EditorGUILayout.FloatField ("Damage Per Second", _target.DamagePerSecond);

        if (Application.isPlaying) {

            

            if (GUILayout.Button("Open")) _target.Open();
            if (GUILayout.Button("Close")) _target.Close();
        }
    }
}
