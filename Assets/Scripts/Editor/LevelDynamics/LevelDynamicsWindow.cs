using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class LevelDynamicsWindow : EditorWindow {
    
    private int unitWidth = 25;
    private int unitHeight = 25;
    private Color pitColor = Color.cyan;
    private Color floorColor = Color.red;
    private Color coverColor = Color.green;
    private int selectedWave;
    private List<WaveTransition> waveTransitions;
    List<string> waveButtonNames;
    GameObject[] levelObjects = null;
    Spawner spawner;

    [MenuItem("Window/Level Dynamics")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelDynamicsWindow));
    }

    private void OnEnable()
    {
        titleContent = new GUIContent("Level Dynamics");
        selectedWave = 0;
        ReadWaveData();
    }

    private void OnGUI()
    {   
        //Context Menu
        if(Event.current.type == EventType.ContextClick)
        {
            Event.current.Use();
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Refresh"), false, ReadWaveData);
            menu.ShowAsContext();
        }

        ShowWaveButtons();
        ShowWaveTriggerOptions();
        GenerateLevelLayout();

        //Apply Button
        if (GUI.Button(new Rect(position.width * 0.8f, position.height * 0.9f, position.width * 0.1f, 30), "Apply Changes"))
        {
            ApplyChanges();
        }
    }

    private void ApplyChanges()
    {
        if (spawner)
        {

        }
    }

    private void ShowWaveButtons()
    {
        selectedWave = GUI.SelectionGrid(new Rect(position.width * 0.025f, 10, position.width * 0.95f, 30), selectedWave, waveButtonNames.ToArray(), waveButtonNames.Count);
    }

    private void ReadWaveData()
    {
        spawner = FindObjectOfType<Spawner>();
        waveButtonNames = new List<string>(spawner.waves.Count);
        waveTransitions = new List<WaveTransition>(spawner.waves.Count);
        for(int i = 0; i < spawner.waves.Count; i++)
        {
            string waveString = "Wave " + (i+1);
            waveButtonNames.Add(waveString);
            waveTransitions.Add(new WaveTransition());
        }        
    }

    private void ShowWaveTriggerOptions()
    {
        GUIStyle fontSizeStyle = new GUIStyle();
        fontSizeStyle.fontSize = 20;
        GUI.Label(new Rect(position.width * 0.4f, 60, 100, 20), "Transition Point", fontSizeStyle);
        GUILayout.BeginHorizontal();
        waveTransitions[selectedWave].triggerStart = GUI.Toggle(new Rect(position.width * 0.42f, 100, 50, 30), waveTransitions[selectedWave].triggerStart, "Start");
        waveTransitions[selectedWave].triggerStart = GUI.Toggle(new Rect(position.width * 0.48f, 100, 50, 30), !waveTransitions[selectedWave].triggerStart, "End");
        GUILayout.EndHorizontal();
    }

    private void GenerateLevelLayout()
    {
        float xOffset = position.width * 0.4f;
        float yOffset = position.height * 0.4f;
        if (levelObjects == null)
        {
            GetLevelObjects();
        }
        else
        {
            List<WaveTransition.LevelUnitStruct> levelUnitsList = waveTransitions[selectedWave].levelUnitsList;
            for (int i = 0; i < levelUnitsList.Count; i++)
            {
                WaveTransition.LevelUnitStruct levelUnit = levelUnitsList[i];
                MeshRenderer renderer = levelUnit.levelUnit.GetComponent<MeshRenderer>();
                if (renderer)
                {                    
                    float meshX = renderer.bounds.size.x;
                    float meshZ = renderer.bounds.size.z;
                    Rect rect = new Rect();
                    rect.x = (levelUnit.levelUnit.transform.localPosition.x / meshX) * unitWidth + xOffset - unitWidth / 2f;
                    rect.y = (levelUnit.levelUnit.transform.localPosition.z / meshZ) * unitHeight + yOffset - unitHeight / 2f;
                    rect.width = unitWidth;
                    rect.height = unitHeight;                    
                    Color defaultColor = GUI.color;
                    if (levelUnit.state == LevelUnitStates.floor)
                    {                        
                        GUI.color = floorColor;
                    }
                    else if (levelUnit.state == LevelUnitStates.cover)
                    {                        
                        GUI.color = coverColor;
                    }
                    else
                    {                        
                        GUI.color = pitColor;
                    }
                    if (GUI.Button(rect, ""))
                    {
                        int localState = (int)levelUnit.state;
                        localState++;
                        if(localState > (int)LevelUnitStates.pit)
                        {
                            localState = 0;
                        }
                        levelUnit.state = (LevelUnitStates)localState;
                        levelUnit.stateChanged = true;
                    }
                    GUI.color = defaultColor;
                    levelUnitsList[i] = levelUnit;
                }
            }
        }
    }

    private void GetLevelObjects()
    {
        int layer = LayerMask.NameToLayer(Strings.Layers.GROUND);
        List<GameObject> returnObjects = new List<GameObject>();
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();        
        foreach (GameObject gameObject in gameObjects)
        {
            if(gameObject.layer == layer)
            {
                returnObjects.Add(gameObject);
            }
        }
        levelObjects = returnObjects.ToArray();
        foreach(WaveTransition waveTransition in waveTransitions)
        {
            waveTransition.levelUnitsList = new List<WaveTransition.LevelUnitStruct>(levelObjects.Length);
            foreach(GameObject levelObject in levelObjects)
            {
                WaveTransition.LevelUnitStruct levelUnitStruct = new WaveTransition.LevelUnitStruct();
                levelUnitStruct.levelUnit = levelObject;
                if (levelObject.transform.localPosition.y == 0f)
                {
                    levelUnitStruct.state = LevelUnitStates.floor;
                }
                else if (levelObject.transform.localPosition.y > 0f)
                {
                    levelUnitStruct.state = LevelUnitStates.cover;
                }
                else
                {
                    levelUnitStruct.state = LevelUnitStates.pit;
                }
                waveTransition.levelUnitsList.Add(levelUnitStruct);
            }
        }        

    }

    private class WaveTransition
    {
        public struct LevelUnitStruct
        {
            public GameObject levelUnit;
            public LevelUnitStates state;
            public bool stateChanged;
        }

        public bool triggerStart = true;
        public List<LevelUnitStruct> levelUnitsList;
    }
}
