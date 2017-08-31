// VFXModChargeEditor.cs
// Author: Aaron

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom editor for VFXModCharge.
/// </summary>
[CustomEditor(typeof(VFXModCharge))]
public sealed class VFXModChargeEditor : Editor
{
    #region Private Fields

    VFXModCharge VFXTarget;

    #endregion
    #region Unity Lifecycle

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (VFXTarget == null) VFXTarget = target as VFXModCharge;

        if (Application.isPlaying)
        {
            if (GUILayout.Button ("Play")) VFXTarget.StartCharging();
            if (GUILayout.Button ("Stop")) VFXTarget.StopCharging();
        }
    }

    #endregion
}
