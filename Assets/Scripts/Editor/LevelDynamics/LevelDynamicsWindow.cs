using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class LevelDynamicsWindow : EditorWindow {

    #region private variables
    private int unitWidth = 25;
    private int unitHeight = 25;
    private Color pitColor = Color.cyan;
    private Color floorColor = Color.red;
    private Color coverColor = Color.green;
    private int selectedWave;
    private List<WaveTransition> waveTransitions;
    private List<string> waveButtonNames;
    private List<GameObject> levelObjects = null;
    private Spawner spawner;
    #endregion

    #region editor lifecycle
    [MenuItem("Level Dynamics/Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelDynamicsWindow));
    }

    private void OnEnable()
    {
        titleContent = new GUIContent("Level Dynamics");
        selectedWave = 0;
        ReadWaveData();
        GetLevelObjects();
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
    #endregion

    #region private functions
    private void ApplyChanges()
    {
        ClearAllEvents();
        if (spawner)
        {
            if (waveTransitions != null)
            {
                for(int i = 0; i < waveTransitions.Count; i++)
                {
                    WaveTransition transition = waveTransitions[i];
                    Wave wave = spawner.waves[i];
                    wave.ClearEvents();
                    string eventName = "Wave " + (i+1);                    
                    for (int j = 0; j < transition.levelUnitsList.Count; j++)
                    {                        
                        WaveTransition.LevelObjectStruct levelUnitStruct = transition.levelUnitsList[j];
                        LevelUnit levelUnit = levelUnitStruct.levelObject.GetComponent<LevelUnit>();
                        if (levelUnit)
                        {                            
                            switch (levelUnitStruct.state)
                            {
                                case LevelUnitStates.cover:
                                    levelUnit.AddCoverStateEvent(eventName);
                                    break;
                                case LevelUnitStates.floor:
                                    levelUnit.AddFloorStateEvent(eventName);
                                    break;
                                case LevelUnitStates.pit:
                                    levelUnit.AddPitStateEvent(eventName);
                                    break;
                            }
                            if (transition.triggerStart)
                            {
                                wave.AddPreEvent(eventName);
                            }
                            else
                            {
                                wave.AddPostEvent(eventName);
                            }
                        }
                    }
                }
            }
        }
    }

    private void ClearAllEvents()
    {
        if (spawner)
        {
            if (waveTransitions != null)
            {
                for (int i = 0; i < waveTransitions.Count; i++)
                {
                    WaveTransition transition = waveTransitions[i];
                    Wave wave = spawner.waves[i];
                    wave.ClearEvents();
                    string eventName = "Wave " + (i + 1);
                    for (int j = 0; j < transition.levelUnitsList.Count; j++)
                    {
                        WaveTransition.LevelObjectStruct levelUnitStruct = transition.levelUnitsList[j];
                        LevelUnit levelUnit = levelUnitStruct.levelObject.GetComponent<LevelUnit>();
                        if (levelUnit)
                        {
                            levelUnit.ClearEvents();
                        }
                    }
                }
            }
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
        bool triggerEnd = GUI.Toggle(new Rect(position.width * 0.48f, 100, 50, 30), !waveTransitions[selectedWave].triggerStart, "End");
        GUILayout.EndHorizontal();
        waveTransitions[selectedWave].triggerStart = !triggerEnd;
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
            List<WaveTransition.LevelObjectStruct> levelUnitsList = waveTransitions[selectedWave].levelUnitsList;
            for (int i = 0; i < levelUnitsList.Count; i++)
            {
                WaveTransition.LevelObjectStruct levelUnitStruct = levelUnitsList[i];
                MeshRenderer renderer = levelUnitStruct.levelObject.GetComponent<MeshRenderer>();
                if (renderer)
                {                    
                    float meshX = renderer.bounds.size.x;
                    float meshZ = renderer.bounds.size.z;
                    Rect rect = new Rect();
                    rect.x = (levelUnitStruct.levelObject.transform.localPosition.x / meshX) * unitWidth + xOffset - unitWidth / 2f;
                    rect.y = (levelUnitStruct.levelObject.transform.localPosition.z / meshZ) * unitHeight + yOffset - unitHeight / 2f;
                    rect.width = unitWidth;
                    rect.height = unitHeight;                    
                    Color defaultColor = GUI.color;
                    if (levelUnitStruct.state == LevelUnitStates.floor)
                    {                        
                        GUI.color = floorColor;
                    }
                    else if (levelUnitStruct.state == LevelUnitStates.cover)
                    {                        
                        GUI.color = coverColor;
                    }
                    else
                    {                        
                        GUI.color = pitColor;
                    }
                    if (GUI.Button(rect, ""))
                    {
                        int localState = (int)levelUnitStruct.state;
                        localState++;
                        if(localState > (int)LevelUnitStates.pit)
                        {
                            localState = 0;
                        }
                        levelUnitStruct.state = (LevelUnitStates)localState;
                        levelUnitStruct.stateChanged = true;
                    }
                    GUI.color = defaultColor;
                    levelUnitsList[i] = levelUnitStruct;
                }
            }
        }
    }

    private void GetLevelObjects()
    {
        int layer = LayerMask.NameToLayer(Strings.Layers.GROUND);
        levelObjects = new List<GameObject>();
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();        
        foreach (GameObject gameObject in gameObjects)
        {
            if(gameObject.layer == layer && gameObject.GetComponent<LevelUnit>())
            {
                levelObjects.Add(gameObject);
            }
        }

        for (int i = 0; i < waveTransitions.Count; i++)
        {
            WaveTransition waveTransition = waveTransitions[i];
            waveTransition.levelUnitsList = new List<WaveTransition.LevelObjectStruct>(levelObjects.Count);
            string eventName = "Wave " + (i+1);
            foreach (GameObject levelObject in levelObjects)
            {
                LevelUnit levelUnit = levelObject.GetComponent<LevelUnit>();
                WaveTransition.LevelObjectStruct levelUnitStruct = new WaveTransition.LevelObjectStruct();
                levelUnitStruct.levelObject = levelObject;
                bool eventExists = false;
                if(levelUnit.CheckForEvent(eventName, LevelUnitStates.floor))
                {
                    levelUnitStruct.state = LevelUnitStates.floor;
                    eventExists = true;
                }
                else if(levelUnit.CheckForEvent(eventName, LevelUnitStates.cover))
                {
                    levelUnitStruct.state = LevelUnitStates.cover;
                    eventExists = true;
                }
                else if(levelUnit.CheckForEvent(eventName, LevelUnitStates.pit))
                {
                    levelUnitStruct.state = LevelUnitStates.pit;
                    eventExists = true;
                }

                if (!eventExists)
                {
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
                }
                waveTransition.levelUnitsList.Add(levelUnitStruct);
            }
        }        

    }
    #endregion

    #region private classes
    private class WaveTransition
    {
        public struct LevelObjectStruct
        {
            public GameObject levelObject;
            public LevelUnitStates state;
            public bool stateChanged;
        }

        public bool triggerStart = true;
        public List<LevelObjectStruct> levelUnitsList;
    }
    #endregion
}
