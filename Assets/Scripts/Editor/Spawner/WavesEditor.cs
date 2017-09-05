using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Spawner)), CanEditMultipleObjects]
public class WavesEditor : Editor
{
    SerializedProperty useIntensityCurve;
    SerializedProperty manualEdits;
    SerializedProperty intensityCurve;
    SerializedProperty waves;

    SerializedProperty currentWaveNumber;
    SerializedProperty currentNumEnemies;

    List<Wave> wavesList;
    GUIStyle intensityTipLabelStyle;    

    void OnEnable()
    {

        currentWaveNumber = serializedObject.FindProperty("currentWaveNumber");
        currentNumEnemies = serializedObject.FindProperty("currentNumEnemies");


        useIntensityCurve = serializedObject.FindProperty("useIntensityCurve");
        manualEdits = serializedObject.FindProperty("manualEdits");
        intensityCurve = serializedObject.FindProperty("intensityCurve");
        waves = serializedObject.FindProperty("waves");
        wavesList = (target as Spawner).waves;
        intensityTipLabelStyle = new GUIStyle();
        intensityTipLabelStyle.wordWrap = true;
        intensityTipLabelStyle.fontStyle = FontStyle.Italic;
        intensityTipLabelStyle.fixedHeight = 30;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(currentWaveNumber);
        EditorGUILayout.PropertyField(currentNumEnemies);
            

        EditorGUILayout.PropertyField(useIntensityCurve, new GUIContent("Use Intensity Curve"));
        bool useIntensity = useIntensityCurve.boolValue;
        bool useManualEdits = !useIntensity && manualEdits.boolValue;

        if (useIntensity)
        {
            GUILayout.Space(10);
            GUILayout.Label("The intensity curve auto-adjusts each wave's intensity and properties", intensityTipLabelStyle);
            EditorGUILayout.PropertyField(intensityCurve);
        }
        else
        {
            EditorGUILayout.PropertyField(manualEdits);
            if (useManualEdits)
            {
                GUILayout.Space(10);
                GUILayout.Label("Disables influence from 'intensity'", intensityTipLabelStyle);
            }
        }
        AdjustWaveIntensity(useIntensity, useManualEdits);
 //       EditorGUI.BeginDisabledGroup(useIntensity);
        EditorGUILayout.PropertyField(waves, true);
 //       EditorGUI.EndDisabledGroup();        

        serializedObject.ApplyModifiedProperties();
    }

    void AdjustWaveIntensity(bool useCurve, bool useManualEdits)
    {
        for (int i = 0; i < wavesList.Count; i++)
        {
            float normalizedTime = (float)i / (wavesList.Count-1);
            float intensity = useCurve ? intensityCurve.animationCurveValue.Evaluate(normalizedTime) : wavesList[i].Intensity;

            if (!useManualEdits)
            {
                wavesList[i].Intensity = intensity;
            }
        }
    }
}