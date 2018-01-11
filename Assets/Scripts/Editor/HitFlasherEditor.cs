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

        if (Application.isPlaying)
        {
            EditorGUILayout.Space();

            if (GUILayout.Button ("Flash"))
            {
                hitFlasher.HitFlash ();
            }

            if (GUILayout.Button("Test Stun Flashing"))
                hitFlasher.SetStunnedState();
        }
    }
}
