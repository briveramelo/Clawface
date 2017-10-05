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
    SerializedProperty waves, spawnType;

    SerializedProperty currentWaveNumber;
    SerializedProperty currentNumEnemies;
    SerializedProperty spawnRange, spawnTimeRange;

    //    SerializedProperty TimeToNextWave;

    List<Wave> wavesList;
    ReorderableList reordList;

    List<bool> waveFoldouts = new List<bool>();
    List<bool> spawnQuantitiesFoldouts = new List<bool>();
    //List<List<bool>> spawnSubQuantitiesFoldouts = new List<List<bool>>();
    const int numEnemies=4;
    float LineHeight { get { return EditorGUIUtility.singleLineHeight+2; } }

    GUIStyle intensityTipLabelStyle;

    void OnEnable()
    {

        currentWaveNumber = serializedObject.FindProperty("currentWaveNumber");
        currentNumEnemies = serializedObject.FindProperty("currentNumEnemies");

        spawnRange = serializedObject.FindProperty("spawnRange");
        spawnTimeRange = serializedObject.FindProperty("spawnTimeRange");


        useIntensityCurve = serializedObject.FindProperty("useIntensityCurve");
        manualEdits = serializedObject.FindProperty("manualEdits");

        intensityCurve = serializedObject.FindProperty("intensityCurve");
        timingCurve = serializedObject.FindProperty("timingCurve");
        spawnType = serializedObject.FindProperty("spawnType");

        waves = serializedObject.FindProperty("waves");
        reordList = new ReorderableList(serializedObject, waves, true, false, true, true);
        reordList.drawElementCallback = DrawWaveProperty;
        reordList.elementHeightCallback = HeightCallback;
        reordList.elementHeight = 12 + LineHeight * 6 + LIST_PADDING;
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
        EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(string.Format("Current Wave Number: {0}", currentWaveNumber.intValue));
            EditorGUILayout.LabelField(string.Format("Current Number of Enemies: {0}", currentNumEnemies.intValue));
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        EditorGUI.EndDisabledGroup();

        GUILayout.Space (LineHeight / 2);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Intensity Settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
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
            }

            AdjustWaveRangeProperties();
            AdjustWaveIntensity(useIntensity, useManualEdits);

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();



        GUILayout.Space (LineHeight / 2);       

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
        GUILayout.Space(3);
        EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spawnRange, true);
            EditorGUILayout.PropertyField(spawnTimeRange, true);
        EditorGUI.indentLevel--;
        GUILayout.Space(5);
        reordList.DoLayoutList();
        EditorGUILayout.EndVertical(); 

        GUILayout.Space (LineHeight);

        serializedObject.ApplyModifiedProperties();
    }

    void DrawWaveProperty(Rect rect, int index, bool isActive, bool isFocused)
    {        
        Rect standardRect = new Rect(rect.position, new Vector2(rect.width, LineHeight)).AddPosition(10, 2.5f);                
        var element = reordList.serializedProperty.GetArrayElementAtIndex(index);
        float startHeightMult = 1;
        waveFoldouts.AddUntil(index);
        waveFoldouts[index] = EditorGUI.Foldout(standardRect, waveFoldouts[index], string.Format("Wave {0}", index), true);
        if (waveFoldouts[index]) {
            EditorGUI.indentLevel++;
            EditorGUI.BeginProperty(rect, GUIContent.none, element);
            EditorGUI.BeginChangeCheck();

            // Intensity            
            EditorGUI.BeginDisabledGroup(useIntensityCurve.boolValue || manualEdits.boolValue);
            if (!manualEdits.boolValue) {
                EditorGUI.PropertyField(standardRect.AddPosition(0, 1 * LineHeight), element.FindPropertyRelative("intensity"));
                startHeightMult = 0;
            }
            else {
                startHeightMult = -1.25f;
            }
            EditorGUI.EndDisabledGroup();

            /*
            // Time to next wave
            EditorGUI.BeginDisabledGroup (useIntensityCurve.boolValue);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y += 2 + LineHeight, rect.width, LineHeight),
                element.FindPropertyRelative("TimeToNextWave"));
            EditorGUI.EndDisabledGroup();
            */

            EditorGUI.BeginDisabledGroup(!manualEdits.boolValue);

            // Total num spawns
            EditorGUI.PropertyField(standardRect.AddPosition(0, (startHeightMult + 2.25f)*LineHeight).AddSize(0, LineHeight), element.FindPropertyRelative("totalNumSpawns"));

            // Spawning time
            EditorGUI.PropertyField(standardRect.AddPosition(0, (startHeightMult + 4.25f) * LineHeight).AddSize(0, LineHeight), element.FindPropertyRelative("spawningTime"));

            // spawn options
            spawnQuantitiesFoldouts.AddUntil(index);
            spawnQuantitiesFoldouts[index] = EditorGUI.Foldout(standardRect.AddPosition(0, (startHeightMult + 6.25f) * LineHeight), spawnQuantitiesFoldouts[index], "Enemy Spawn Quantities", true);
            if (spawnQuantitiesFoldouts[index]) {
                EditorGUI.indentLevel++;
                
                DrawSubQuantities(element.FindPropertyRelative("enemySpawnQuantities"), standardRect.AddPosition(0, (startHeightMult + 7.25f) * LineHeight), index);

                
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.EndChangeCheck();
            EditorGUI.EndProperty();
            EditorGUI.indentLevel--;
        }        
    }

    void DrawSubQuantities(SerializedProperty quantities, Rect rect, int index) {
        //Vector2 closedExtra = new Vector2(0, LineHeight * 1);
        //Vector2 openExtra = new Vector2(0, LineHeight * 3);
        //System.Func<int, Vector2> GetHeight = (subIndex) => {
        //    int previousOpened = 0;
        //    int previousClosed = 0;
        //    for (int i=0; i< spawnSubQuantitiesFoldouts[index].Count; i++) {
        //        if (subIndex > i) {
        //            if (spawnSubQuantitiesFoldouts[index][i]) {
        //                previousOpened++;
        //            }
        //            else{
        //                previousClosed++;
        //            }
        //        }
        //    }
        //    Vector2 totalHeights = closedExtra * previousClosed + openExtra * previousOpened;
        //    return totalHeights;
        //};

        //spawnSubQuantitiesFoldouts.AddNewUntil(index);
        //spawnSubQuantitiesFoldouts[index].AddUntil(numEnemies);
        DrawSubQuantity(quantities, "blaster", rect.AddPosition(0, 0 * LineHeight), index, 0);
        DrawSubQuantity(quantities, "bouncer", rect.AddPosition(0, 1 * LineHeight), index, 1);
        DrawSubQuantity(quantities, "kamikaze", rect.AddPosition(0, 2 * LineHeight), index, 2);
        DrawSubQuantity(quantities, "zombie", rect.AddPosition(0, 3 * LineHeight), index, 3);
    }

    void DrawSubQuantity(SerializedProperty quantities, string propertyRelative, Rect rect, int index, int subIndex) {
        SerializedProperty subQuant = quantities.FindPropertyRelative(propertyRelative);
        SerializedProperty type = subQuant.FindPropertyRelative("spawnType");
        SerializedProperty spawnCount = subQuant.FindPropertyRelative("spawnCount");
        string name = ((SpawnType)type.enumValueIndex).ToString();

        spawnCount.intValue = EditorGUI.IntField(rect, name, spawnCount.intValue);

        //spawnSubQuantitiesFoldouts[index][subIndex] = EditorGUI.Foldout(rect, spawnSubQuantitiesFoldouts[index][subIndex], name, true);
        //if (spawnSubQuantitiesFoldouts[index][subIndex]) {
        //    EditorGUI.indentLevel++;
        //    //EditorGUI.PropertyField(rect.AddPosition(0, LineHeight), type);
        //    EditorGUI.PropertyField(rect.AddPosition(0, LineHeight), spawnCount);
        //    EditorGUI.indentLevel--;
        //}
    }

    float HeightCallback(int index) {
        
        Repaint();
        float height = LineHeight + 5;
        waveFoldouts.AddUntil(index);
        spawnQuantitiesFoldouts.AddUntil(index);
        if (waveFoldouts[index]) {
            int lineCount = 6;
            height += lineCount * LineHeight;
            if (spawnQuantitiesFoldouts[index]) {
                int itemCount = numEnemies;
                height += itemCount * LineHeight;
                //spawnSubQuantitiesFoldouts.AddNewUntil(index);
                //spawnSubQuantitiesFoldouts[index].AddUntil(numEnemies);
                //height += spawnSubQuantitiesFoldouts[index].FindAll(item => true).Count * LineHeight;
            }
            if (useIntensityCurve.boolValue || manualEdits.boolValue) {
                height -= LineHeight;
            }
        }


        return height;
        
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

    void AdjustWaveRangeProperties() {
        for (int i = 0; i < wavesList.Count; i++) {
            wavesList[i].totalNumSpawns.minLimit = spawnRange.FindPropertyRelative("min").intValue;
            wavesList[i].totalNumSpawns.maxLimit = spawnRange.FindPropertyRelative("max").intValue;
            wavesList[i].spawnOffset = Mathf.RoundToInt(spawnRange.FindPropertyRelative("rangeSize").intValue/2);

            wavesList[i].spawningTime.minLimit = spawnTimeRange.FindPropertyRelative("min").floatValue;
            wavesList[i].spawningTime.maxLimit = spawnTimeRange.FindPropertyRelative("max").floatValue;
            wavesList[i].spawnTimeOffset = spawnTimeRange.FindPropertyRelative("rangeSize").floatValue/2;
        }
    }
   
}