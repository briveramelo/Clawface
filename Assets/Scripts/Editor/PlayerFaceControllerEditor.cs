using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Turing.VFX;

[CustomEditor(typeof(PlayerFaceController))]
public class PlayerFaceControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            PlayerFaceController face = target as PlayerFaceController;

            if(GUILayout.Button("Force Angry"))
                face.SetTemporaryEmotion (PlayerFaceController.Emotion.Angry, 1f);

            if (GUILayout.Button ("Force Happy"))
                face.SetTemporaryEmotion (PlayerFaceController.Emotion.Happy, 1f);
        }
    }
}
