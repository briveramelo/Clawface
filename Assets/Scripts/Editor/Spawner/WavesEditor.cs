// WaveEditor.cs
// Author: Lai, Aaron

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(Spawner)), CanEditMultipleObjects]
public class WavesEditor : Editor
{
    const float LIST_PADDING = 8;

    SerializedProperty useIntensityCurve;
    SerializedProperty manualEdits;
    SerializedProperty intensityCurve;
    SerializedProperty timingCurve;
    SerializedProperty waves;

    SerializedProperty currentWaveNumber;
    SerializedProperty currentNumEnemies;
//    SerializedProperty TimeToNextWave;

    List<Wave> wavesList;
    ReorderableList reordList;

    GUIStyle intensityTipLabelStyle;

    void OnEnable()
    {

        currentWaveNumber = serializedObject.FindProperty("currentWaveNumber");
        currentNumEnemies = serializedObject.FindProperty("currentNumEnemies");
//        TimeToNextWave = serializedObject.FindProperty("TimeToNextWave");

        useIntensityCurve = serializedObject.FindProperty("useIntensityCurve");
        manualEdits = serializedObject.FindProperty("manualEdits");

        intensityCurve = serializedObject.FindProperty("intensityCurve");
        timingCurve = serializedObject.FindProperty("timingCurve");

        waves = serializedObject.FindProperty("waves");
        reordList = new ReorderableList(serializedObject, waves, true, false, true, true);
        reordList.drawElementCallback = DrawWaveProperty;
        reordList.elementHeight = 12 + EditorGUIUtility.singleLineHeight * 6 + LIST_PADDING;
        wavesList = (target as Spawner).waves;
        intensityTipLabelStyle = new GUIStyle();
        intensityTipLabelStyle.wordWrap = true;
        intensityTipLabelStyle.fontStyle = FontStyle.Italic;
        intensityTipLabelStyle.fixedHeight = 30;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Runtime status
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Runtime Status", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(string.Format("Current Wave Number: {0}", currentWaveNumber.intValue));
        EditorGUILayout.LabelField(string.Format("Current Number of Enemies: {0}", currentNumEnemies.intValue));
//        EditorGUILayout.LabelField(string.Format("Time to Next Wave: {0}", TimeToNextWave.floatValue));
        EditorGUILayout.EndVertical();

        EditorGUI.EndDisabledGroup();

        GUILayout.Space (EditorGUIUtility.singleLineHeight / 2);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Intensity Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(useIntensityCurve, new GUIContent("Use Intensity Curve"));
        bool useIntensity = useIntensityCurve.boolValue;
        bool useManualEdits = !useIntensity && manualEdits.boolValue;

        if (useIntensity)
        {
            GUILayout.Space(10);
            GUILayout.Label("The intensity curve auto-adjusts each wave's intensity and properties", intensityTipLabelStyle);
            EditorGUILayout.PropertyField(intensityCurve);

//          GUILayout.Space(10);
//          GUILayout.Label("The timing curve auto-adjusts each wave's time to next wave", intensityTipLabelStyle);
//          EditorGUILayout.PropertyField(timingCurve);
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
 //       AdjustWaveTime(useIntensity, useManualEdits);

        EditorGUILayout.EndVertical();

        GUILayout.Space (EditorGUIUtility.singleLineHeight / 2);

        //       EditorGUI.BeginDisabledGroup(useIntensity);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Waves", EditorStyles.boldLabel);

        if (useIntensityCurve.boolValue)
            EditorGUILayout.LabelField (
                "Wave properties are being controlled by the intensity curve.", 
                EditorStyles.helpBox);

        else
        {
            if (!manualEdits.boolValue)
            EditorGUILayout.LabelField (
                "Wave properties are being controlled by intensity values.", 
                EditorStyles.helpBox);

            else
                EditorGUILayout.LabelField (
                "Intensity slider is disabled (manual edits on).", 
                EditorStyles.helpBox);
        }

        reordList.DoLayoutList();
        EditorGUILayout.EndVertical(); 

        GUILayout.Space (EditorGUIUtility.singleLineHeight);

        serializedObject.ApplyModifiedProperties();
    }

    void DrawWaveProperty(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = reordList.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.BeginProperty(rect, GUIContent.none, element);
        EditorGUI.BeginChangeCheck();

        // Intensity
        EditorGUI.BeginDisabledGroup(manualEdits.boolValue || useIntensityCurve.boolValue);
        EditorGUI.PropertyField(new Rect(rect.x, rect.y += 2, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("intensity"));
        EditorGUI.EndDisabledGroup();

        /*
        // Time to next wave
        EditorGUI.BeginDisabledGroup (useIntensityCurve.boolValue);
        EditorGUI.PropertyField(new Rect(rect.x, rect.y += 2 + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("TimeToNextWave"));
        EditorGUI.EndDisabledGroup();
        */

        EditorGUI.BeginDisabledGroup(!manualEdits.boolValue);

        // Total num spawns
        EditorGUI.PropertyField(new Rect(rect.x, rect.y += 2 + EditorGUIUtility.singleLineHeight, rect.width, 2 * EditorGUIUtility.singleLineHeight + 2),
            element.FindPropertyRelative("totalNumSpawns"));

        // Spawning time
        EditorGUI.PropertyField(new Rect(rect.x, rect.y += 4 + EditorGUIUtility.singleLineHeight * 2, rect.width, 2 * EditorGUIUtility.singleLineHeight + 2),
            element.FindPropertyRelative("SpawningTime"));

        EditorGUI.EndDisabledGroup();

        EditorGUI.EndChangeCheck();
        EditorGUI.EndProperty();
    }

    void AdjustWaveIntensity(bool useCurve, bool useManualEdits)
    {
        for (int i = 0; i < wavesList.Count; i++)
        {
            float normalizedTime = (float)i / (wavesList.Count - 1);
            float intensity = useCurve ? intensityCurve.animationCurveValue.Evaluate(normalizedTime) : wavesList[i].Intensity;

            if (!useManualEdits)
            {
                wavesList[i].Intensity = intensity;
            }
        }
    }


    /*
    void AdjustWaveTime(bool useCurve, bool useManualEdits)
    {
        for (int i = 0; i < wavesList.Count; i++)
        {
            float normalizedTime = (float)i / (wavesList.Count - 1);
            float time = useCurve ? timingCurve.animationCurveValue.Evaluate(normalizedTime) : wavesList[i].Time;

            if (!useManualEdits)
            {
                wavesList[i].Time = time;
            }
        }
    }
    */

}