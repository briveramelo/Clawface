// FireTrapEditor.cs
// Author: Aaron

using Turing.Gameplay;

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom editor for FireTraps.
/// </summary>
[CustomEditor(typeof(FireTrap))]
public sealed class FireTrapEditor : Editor
{
    #region Unity Lifecycle

    public override void OnInspectorGUI()
    {
        var FTTarget = target as FireTrap;

        EditorGUILayout.Space();

        FTTarget.TrapMode = (FireTrap.Mode)EditorGUILayout.EnumPopup ("Trap Mode", FTTarget.TrapMode);

        switch (FTTarget.TrapMode)
        {
            case FireTrap.Mode.ContinuousOpenClose:
                FTTarget.OpenTime = EditorGUILayout.FloatField ("Open Time", FTTarget.OpenTime);
                FTTarget.CloseTime = EditorGUILayout.FloatField ("Close Time", FTTarget.CloseTime);
                FTTarget.StayOpenTime = EditorGUILayout.FloatField ("Stay Open Time", FTTarget.StayOpenTime);
                FTTarget.StayClosedTime = EditorGUILayout.FloatField ("Stay Closed Time", FTTarget.StayClosedTime);
                break;

            case FireTrap.Mode.ContinuousStream:
                break;

            case FireTrap.Mode.PressureTrigger:
                FTTarget.OpenTime = EditorGUILayout.FloatField ("Open Time", FTTarget.OpenTime);
                FTTarget.CloseTime = EditorGUILayout.FloatField ("Close Time", FTTarget.CloseTime);
                break;
        }

        FTTarget.DamagePerSecond = EditorGUILayout.FloatField ("Damage Per Second", FTTarget.DamagePerSecond);

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Open")) FTTarget.Open();
            if (GUILayout.Button("Close")) FTTarget.Close();
        }
    }

    #endregion
}
