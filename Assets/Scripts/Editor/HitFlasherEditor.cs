// HitFlasherEditor.cs
// Author: Aaron

using Turing.VFX;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(HitFlasher))]
public sealed class HitFlasherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        HitFlasher hitFlasher = target as HitFlasher;

        EditorGUILayout.Space();

        if (GUILayout.Button ("Flash"))
        {
            hitFlasher.Flash (2.0f, 0.15f);
        }
    }
}
