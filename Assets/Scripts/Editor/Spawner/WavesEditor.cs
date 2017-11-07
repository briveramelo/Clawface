// WaveEditor.cs
// Author: Lai, Aaron, Brandon

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using ModMan;

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
    SerializedProperty totalNumEnemies;

    List<Wave> wavesList;
    ReorderableList reordList;
    ReorderableList test;

    GUIStyle intensityTipLabelStyle;

    void OnEnable()
    {

        currentWaveNumber = serializedObject.FindProperty("currentWaveNumber");
        currentNumEnemies = serializedObject.FindProperty("currentNumEnemies");
        totalNumEnemies = serializedObject.FindProperty("totalNumEnemies");

        useIntensityCurve = serializedObject.FindProperty("useIntensityCurve");
        manualEdits = serializedObject.FindProperty("manualEdits");

        intensityCurve = serializedObject.FindProperty("intensityCurve");
        timingCurve = serializedObject.FindProperty("timingCurve");

        waves = serializedObject.FindProperty("waves");


        reordList = new ReorderableList(serializedObject, waves, true, false, true, true);
        reordList.drawElementCallback = DrawWaveProperty;

        reordList.elementHeight = 12 + EditorGUIUtility.singleLineHeight * 16 + LIST_PADDING;

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
        EditorGUILayout.LabelField(string.Format("Total Number of Enemies: {0}", totalNumEnemies.intValue));

        EditorGUILayout.EndVertical();

        EditorGUI.EndDisabledGroup();

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

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

        GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

        //       EditorGUI.BeginDisabledGroup(useIntensity);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Waves", EditorStyles.boldLabel);

        if (useIntensityCurve.boolValue)
            EditorGUILayout.LabelField(
                "Wave properties are being controlled by the intensity curve.",
                EditorStyles.helpBox);

        else
        {
            if (!manualEdits.boolValue)
                EditorGUILayout.LabelField(
                    "Wave properties are being controlled by intensity values.",
                    EditorStyles.helpBox);

            else
                EditorGUILayout.LabelField(
                "Intensity slider is disabled (manual edits on).",
                EditorStyles.helpBox);
        }

        reordList.DoLayoutList();
        EditorGUILayout.EndVertical();

        GUILayout.Space(EditorGUIUtility.singleLineHeight);

        serializedObject.ApplyModifiedProperties();

    }

    void DrawWaveProperty(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = reordList.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.BeginProperty(rect, GUIContent.none, element);
        EditorGUI.BeginChangeCheck();

        // Intensity
        EditorGUI.BeginDisabledGroup(manualEdits.boolValue || useIntensityCurve.boolValue);
        EditorGUI.PropertyField(new Rect(rect.x, rect.y += 2, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("intensity"));
        EditorGUI.EndDisabledGroup();

        EditorGUI.PropertyField(new Rect(rect.x, rect.y += 2 + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("NumToNextWave"));

        EditorGUI.BeginDisabledGroup(!manualEdits.boolValue);

   
        /*
        // Total num spawns
        EditorGUI.PropertyField(new Rect(rect.x, rect.y += 2 + EditorGUIUtility.singleLineHeight, rect.width, 2 * EditorGUIUtility.singleLineHeight + 2),
            element.FindPropertyRelative("totalNumSpawn"));
 
        // Spawning time
        EditorGUI.PropertyField(new Rect(rect.x, rect.y += 4 + EditorGUIUtility.singleLineHeight * 2, rect.width, 2 * EditorGUIUtility.singleLineHeight + 2),
            element.FindPropertyRelative("SpawningTime"));
        */

        EditorGUI.EndDisabledGroup();

        SerializedProperty monsterListProp = element.FindPropertyRelative("monsterList");
        ReorderableList monsterList = new ReorderableList(serializedObject, monsterListProp, true, false, true, true);


        monsterList.drawElementCallback = (Rect i_Rect, int i_index, bool i_isActive, bool i_isFocused) =>
        {
            var i_element = monsterList.serializedProperty.GetArrayElementAtIndex(i_index);
            EditorGUI.PropertyField(new Rect(i_Rect.x, i_Rect.y, 60, EditorGUIUtility.singleLineHeight), i_element.FindPropertyRelative("Type"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(i_Rect.x + i_Rect.width - 30, i_Rect.y, 30, EditorGUIUtility.singleLineHeight), i_element.FindPropertyRelative("Count"), GUIContent.none);
        };


        monsterList.onAddCallback = (ReorderableList i_List) =>
        {
            if(i_List.count >= 4)
            {
                EditorUtility.DisplayDialog("Warning!", "You can't add more types", "OK");
            }
            else
            {
                var i_index = i_List.serializedProperty.arraySize;
                i_List.serializedProperty.arraySize++;
                i_List.index = i_index;

                var i_element = i_List.serializedProperty.GetArrayElementAtIndex(i_index);
                i_element.FindPropertyRelative("Type").enumValueIndex = 0;
                i_element.FindPropertyRelative("Count").intValue = 1;
            }
        };


        monsterList.DoList(new Rect(rect.x, rect.y += 4 + EditorGUIUtility.singleLineHeight * 3, rect.width, 8 * EditorGUIUtility.singleLineHeight));

        EditorGUI.EndChangeCheck();
        EditorGUI.EndProperty();
    }


    void AdjustWaveIntensity(bool useCurve, bool useManualEdits)
    {
        float normalizedTime = (float)1 / (wavesList.Count + 1);

        for (int i = 0; i < wavesList.Count; i++)
        {

            float intensity = useCurve ? intensityCurve.animationCurveValue.Evaluate(normalizedTime) : wavesList[i].Intensity;

            if (!useManualEdits)
            {
                wavesList[i].Intensity = intensity;

                for(int j = 0; j < wavesList[i].monsterList.Count; j++)
                {
                    int number = useCurve ? (int)(intensityCurve.animationCurveValue.Evaluate(normalizedTime) * 10.0f) : wavesList[i].monsterList[j].Count;
                    wavesList[i].monsterList[j].Count = number;
                }
            }

            normalizedTime += normalizedTime;
        }
    }
}