using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.Assertions;

public class LevelDynamicsWindow : EditorWindow {

    #region private variables
    private int unitWidth = 25;
    private int unitHeight = 25;
    private Color pitColor = Color.cyan;
    private Color floorColor = Color.red;
    private Color coverColor = Color.green;
    private int selectedSpawner;
    private int selectedWave;
    private List<SpawnerObject> spawnerObjects;
    private List<string> spawnerButtonNames;
    private List<GameObject> levelObjects = null;
    private SpawnManager spawnManager;
    private MusicIntensityManager musicIntensityManager;
    private static string spawnerNumberString = "#spawnerNumber";
    private static string waveNumberString = "#waveNumber";
    private static string eventName = "Spawner "+ spawnerNumberString + " Wave " + waveNumberString;
    private bool isReady;
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
        selectedSpawner = 0;
        selectedWave = 0;
        Init();
    }

    private void Init()
    {
        isReady = ReadWaveData();
        if (isReady)
        {
            GetLevelObjects();
        }
    }

    private void OnGUI()
    {
        if (isReady)
        {
            //Context Menu
            if (Event.current.type == EventType.ContextClick)
            {
                Event.current.Use();
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Refresh"), false, Init);
                menu.ShowAsContext();
            }

            ShowWaveButtons();
            ShowWaveTriggerOptions();
            GenerateLevelLayout();
            ShowMusicTrackSlot();

            //Copy Button
            if (GUI.Button(new Rect(position.width * 0.8f, position.height * 0.6f, position.width * 0.11f, 30), "Copy Last State"))
            {
                CopyLastState();
            }

            //Apply Button
            if (GUI.Button(new Rect(position.width * 0.8f, position.height * 0.85f, position.width * 0.1f, 30), "Bake Data"))
            {
                BakeData();
            }

            //Clear Button
            if (GUI.Button(new Rect(position.width * 0.8f, position.height * 0.9f, position.width * 0.1f, 30), "Clear Data"))
            {
                ClearAllEvents();
                Init();
            }
        }
    }
    #endregion

    #region private functions
    private void ShowMusicTrackSlot()
    {        
        AudioClip selectedClip = spawnerObjects[selectedSpawner].waveObjects[selectedWave].audioClip;
        GUI.Label(new Rect(0.8f * position.width, 110 - 16f, position.width * 0.15f, 16f), "Music Track");
        selectedClip = (AudioClip)EditorGUI.ObjectField(new Rect(0.8f * position.width, 110, position.width * 0.15f, 16f), selectedClip, typeof(AudioClip), true);
        spawnerObjects[selectedSpawner].waveObjects[selectedWave].audioClip = selectedClip;
    }

    private void CopyLastState()
    {
        if(selectedWave > 0)
        {
            int previousWave = selectedWave - 1;
            WaveObject previousWaveObject = spawnerObjects[selectedSpawner].waveObjects[previousWave];
            WaveObject selectedWaveObject = spawnerObjects[selectedSpawner].waveObjects[selectedWave];
            for(int i=0;i< selectedWaveObject.levelUnitsList.Count; i++)
            {
                WaveObject.LevelObjectData levelObjectData = selectedWaveObject.levelUnitsList[i];
                levelObjectData.state = previousWaveObject.levelUnitsList[i].state;
                selectedWaveObject.levelUnitsList[i] = levelObjectData;
            }
            //selectedWaveObject.audioClip = previousWaveObject.audioClip;
        }
        else if(selectedSpawner > 0)
        {
            int previousSpawner = selectedSpawner - 1;
            int previousWave = spawnerObjects[previousSpawner].waveObjects.Count - 1;
            WaveObject previousWaveObject = spawnerObjects[previousSpawner].waveObjects[previousWave];
            WaveObject selectedWaveObject = spawnerObjects[selectedSpawner].waveObjects[selectedWave];
            for (int i = 0; i < selectedWaveObject.levelUnitsList.Count; i++)
            {
                WaveObject.LevelObjectData levelObjectData = selectedWaveObject.levelUnitsList[i];
                levelObjectData.state = previousWaveObject.levelUnitsList[i].state;
                selectedWaveObject.levelUnitsList[i] = levelObjectData;
            }
        }
    }

    private void BakeData()
    {
        ClearAllEvents();
        if (spawnManager)
        {
            if (spawnerObjects != null)
            {
                for(int i = 0; i < spawnerObjects.Count; i++)
                {
                    if(spawnerObjects[i].waveObjects != null)
                    {
                        Spawner spawner = spawnManager.spawners[i].Prefab.GetComponent<Spawner>();
                        for (int j=0;j< spawnerObjects[i].waveObjects.Count; j++)
                        {
                            // Set level unit triggers
                            WaveObject waveObject = spawnerObjects[i].waveObjects[j];                            
                            string localEventName = eventName.Replace(spawnerNumberString, (i + 1).ToString()).Replace(waveNumberString, (j + 1).ToString());
                            if (waveObject.triggerStart)
                            {
                                spawner.waves[j].AddPreEvent(localEventName);
                            }
                            else
                            {
                                spawner.waves[j].AddPostEvent(localEventName);
                            }
                            for (int k = 0; k < waveObject.levelUnitsList.Count; k++)
                            {
                                WaveObject.LevelObjectData levelUnitStruct = waveObject.levelUnitsList[k];
                                LevelUnit levelUnit = levelUnitStruct.levelUnit.GetComponent<LevelUnit>();
                                if (levelUnit)
                                {
                                    switch (levelUnitStruct.state)
                                    {
                                        case LevelUnitStates.cover:
                                            levelUnit.AddCoverStateEvent(localEventName);
                                            break;
                                        case LevelUnitStates.floor:
                                            levelUnit.AddFloorStateEvent(localEventName);
                                            break;
                                        case LevelUnitStates.pit:
                                            levelUnit.AddPitStateEvent(localEventName);
                                            break;
                                    }                                    
                                }
                            }

                            // Set music triggers
                            if (waveObject.audioClip != null) {                                
                                string musicEventName = "Music_" + localEventName;
                                if (waveObject.triggerStart)
                                {
                                    spawner.waves[j].AddPreEvent(musicEventName);
                                }
                                else
                                {
                                    spawner.waves[j].AddPostEvent(musicEventName);
                                }                                
                                musicIntensityManager.AddMusicTransitionEvent(musicEventName, waveObject.audioClip);
                            }
                            EditorUtility.SetDirty(musicIntensityManager);
                        }
                        EditorUtility.SetDirty(spawner);
                    }                    
                }
            }
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private void ClearAllEvents()
    {
        if (spawnManager)
        {
            if (spawnerObjects != null)
            {
                for (int i = 0; i < spawnerObjects.Count; i++)
                {
                    Spawner spawner = spawnManager.spawners[i].Prefab.GetComponent<Spawner>();
                    SpawnerObject spawnerObject = spawnerObjects[i];
                    for (int j = 0; j < spawnerObject.waveObjects.Count; j++)
                    {
                        Wave wave = spawner.waves[j];
                        wave.ClearEvents();
                        WaveObject waveObject = spawnerObject.waveObjects[j];
                        for (int k = 0; k < waveObject.levelUnitsList.Count; k++)
                        {
                            
                            WaveObject.LevelObjectData levelUnitStruct = waveObject.levelUnitsList[k];
                            LevelUnit levelUnit = levelUnitStruct.levelUnit.GetComponent<LevelUnit>();
                            if (levelUnit)
                            {
                                levelUnit.ClearEvents();
                            }
                        }
                    }
                }
            }
        }
        musicIntensityManager.ClearAll();
    }

    private void ShowWaveButtons()
    {
        int localSelectedSpawner = GUI.SelectionGrid(new Rect(position.width * 0.025f, 10, position.width * 0.95f, 30), selectedSpawner, spawnerButtonNames.ToArray(), spawnerButtonNames.Count);
        if(localSelectedSpawner != selectedSpawner)
        {
            selectedSpawner = localSelectedSpawner;
            selectedWave = 0;
        }
        selectedWave = GUI.SelectionGrid(new Rect(position.width * 0.2f, 50, position.width * 0.6f, 20), selectedWave, spawnerObjects[selectedSpawner].waveButtonNames.ToArray(), spawnerObjects[selectedSpawner].waveButtonNames.Count);
    }

    private bool ReadWaveData()
    {
        spawnManager = FindObjectOfType<SpawnManager>();        
        musicIntensityManager = FindObjectOfType<MusicIntensityManager>();
        if (spawnManager && musicIntensityManager)
        {
            spawnerButtonNames = new List<string>(spawnManager.spawners.Count);
            spawnerObjects = new List<SpawnerObject>(spawnManager.spawners.Count);
            for (int i = 0; i < spawnManager.spawners.Count; i++)
            {
                Spawner spawner = spawnManager.spawners[i].Prefab.GetComponent<Spawner>();
                if (spawner)
                {
                    spawnerButtonNames.Add("Spawner " + (i + 1));
                    SpawnerObject spawnerObject = new SpawnerObject();
                    spawnerObject.waveButtonNames = new List<string>(spawner.waves.Count);
                    spawnerObject.waveObjects = new List<WaveObject>(spawner.waves.Count);
                    for (int j = 0; j < spawner.waves.Count; j++)
                    {
                        Wave wave = spawner.waves[j];
                        spawnerObject.waveButtonNames.Add("Wave " + (j + 1));
                        WaveObject waveObject = new WaveObject();
                        string musicEventName = "Music_" + eventName.Replace(spawnerNumberString, (i + 1).ToString()).Replace(waveNumberString, (j + 1).ToString());
                        waveObject.audioClip = musicIntensityManager.GetAudioClipByEventName(musicEventName);
                        spawnerObject.waveObjects.Add(waveObject);
                    }
                    spawnerObjects.Add(spawnerObject);
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ShowWaveTriggerOptions()
    {
        GUIStyle fontSizeStyle = new GUIStyle();
        fontSizeStyle.fontSize = 20;
        GUI.Label(new Rect(position.width * 0.4f, 80, 100, 20), "Transition Point", fontSizeStyle);
        WaveObject selectedWaveObject = spawnerObjects[selectedSpawner].waveObjects[selectedWave];
        GUILayout.BeginHorizontal();
        selectedWaveObject.triggerStart = GUI.Toggle(new Rect(position.width * 0.42f, 110, 50, 30), selectedWaveObject.triggerStart, "Start");
        bool triggerEnd = GUI.Toggle(new Rect(position.width * 0.48f, 110, 50, 30), !selectedWaveObject.triggerStart, "End");
        GUILayout.EndHorizontal();
        selectedWaveObject.triggerStart = !triggerEnd;
    }

    private void GenerateLevelLayout()
    {
        float xOffset = position.width * 0.4f;
        float yOffset = position.height * 0.8f;
        if (levelObjects == null)
        {
            GetLevelObjects();
        }
        else
        {
            List<WaveObject.LevelObjectData> levelUnitsList = spawnerObjects[selectedSpawner].waveObjects[selectedWave].levelUnitsList;
            for (int i = 0; i < levelUnitsList.Count; i++)
            {
                WaveObject.LevelObjectData levelUnitStruct = levelUnitsList[i];
                MeshRenderer renderer = levelUnitStruct.levelUnit.GetComponent<MeshRenderer>();
                if (renderer)
                {                    
                    float meshX = renderer.bounds.size.x;
                    float meshZ = renderer.bounds.size.z;
                    Rect rect = new Rect();
                    rect.x = (levelUnitStruct.levelUnit.transform.localPosition.x / meshX) * unitWidth + xOffset - unitWidth / 2f;
                    rect.y = (-levelUnitStruct.levelUnit.transform.localPosition.z / meshZ) * unitHeight + yOffset - unitHeight / 2f;
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

        for(int i = 0; i < spawnerObjects.Count; i++)
        {
            SpawnerObject spawnerObject = spawnerObjects[i];            
            for (int j = 0; j < spawnerObject.waveObjects.Count; j++)
            {
                WaveObject waveObject = spawnerObject.waveObjects[j];
                waveObject.levelUnitsList = new List<WaveObject.LevelObjectData>(levelObjects.Count);
                string localEventName = eventName.Replace(spawnerNumberString, (i + 1).ToString()).Replace(waveNumberString, (j + 1).ToString());
                foreach (GameObject levelObject in levelObjects)
                {
                    LevelUnit levelUnit = levelObject.GetComponent<LevelUnit>();
                    WaveObject.LevelObjectData levelUnitStruct = new WaveObject.LevelObjectData();
                    levelUnitStruct.levelUnit = levelUnit;
                    bool eventExists = false;
                    if (levelUnit.CheckForEvent(localEventName, LevelUnitStates.floor))
                    {
                        levelUnitStruct.state = LevelUnitStates.floor;
                        eventExists = true;
                    }
                    else if (levelUnit.CheckForEvent(localEventName, LevelUnitStates.cover))
                    {
                        levelUnitStruct.state = LevelUnitStates.cover;
                        eventExists = true;
                    }
                    else if (levelUnit.CheckForEvent(localEventName, LevelUnitStates.pit))
                    {
                        levelUnitStruct.state = LevelUnitStates.pit;
                        eventExists = true;
                    }

                    if (!eventExists)
                    {                       
                        levelUnitStruct.state = levelUnit.defaultState;
                    }
                    waveObject.levelUnitsList.Add(levelUnitStruct);
                }
            }
        }
    }
    #endregion

    #region private classes    
    private class WaveObject
    {
        public struct LevelObjectData
        {
            public LevelUnit levelUnit;
            public LevelUnitStates state;
            public bool stateChanged;
        }

        public bool triggerStart = true;
        public List<LevelObjectData> levelUnitsList;
        public AudioClip audioClip;
    }

    private class SpawnerObject
    {
        public List<string> waveButtonNames;
        public List<WaveObject> waveObjects;
    }
    #endregion
}
