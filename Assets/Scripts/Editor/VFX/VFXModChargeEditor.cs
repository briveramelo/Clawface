using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VFXModCharge))]
public class VFXModChargeEditor : Editor {

    VFXModCharge _target;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (_target == null) _target = target as VFXModCharge;

        if (Application.isPlaying) {
            if (GUILayout.Button ("Play")) _target.StartCharging();
            if (GUILayout.Button ("Stop")) _target.StopCharging();
        }
    }
}
