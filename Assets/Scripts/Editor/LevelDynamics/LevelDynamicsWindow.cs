using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class LevelDynamicsWindow : EditorWindow {

    private int selectedLayer = 0;
    private string[] layerNames = { "Ground" };
    private int unitWidth = 25;
    private int unitHeight = 25;
    private Color pitColor = Color.cyan;
    private Color floorColor = Color.red;
    private Color coverColor = Color.green;
    private int selectedWave;

    [MenuItem("Window/Level Dynamics")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelDynamicsWindow));
    }

    private void OnEnable()
    {
        titleContent = new GUIContent("Level Dynamics");
        selectedWave = 0;
    }    

    private void OnGUI()
    {
        ReadWaveData();
        GenerateLevelLayout();
    }

    private void ReadWaveData()
    {
        Spawner spawner = FindObjectOfType<Spawner>();
        List<string> buttonNames = new List<string>();
        for(int i = 0; i < spawner.waves.Count; i++)
        {
            string waveString = "Wave " + i;
            buttonNames.Add(waveString);
        }
        selectedWave = GUI.SelectionGrid(new Rect(position.width * 0.025f, 10, position.width * 0.95f, 30), selectedWave, buttonNames.ToArray(), buttonNames.Count);
        Debug.Log(selectedWave);
    }

    private void GenerateLevelLayout()
    {
        float xOffset = position.width * 0.4f;
        float yOffset = position.height * 0.4f;
        GameObject[] gameObjects = GetObjectsWithLayer(layerNames[selectedLayer]);
        foreach (GameObject gameObject in gameObjects)
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            if (renderer)
            {
                float meshX = renderer.bounds.size.x;
                float meshZ = renderer.bounds.size.z;
                Rect rect = new Rect();
                rect.x = (gameObject.transform.localPosition.x / meshX) * unitWidth + xOffset - unitWidth / 2f;
                rect.y = (gameObject.transform.localPosition.z / meshZ) * unitHeight + yOffset - unitHeight / 2f;
                rect.width = unitWidth;
                rect.height = unitHeight;
                Color defaultColor = GUI.color;
                if (gameObject.transform.localPosition.y == 0f)
                {
                    GUI.color = floorColor;
                }
                else if (gameObject.transform.localPosition.y > 0f)
                {
                    GUI.color = coverColor;
                }
                else
                {
                    GUI.color = pitColor;
                }
                if (GUI.Button(rect, ""))
                {
                    //Do stuff here
                }
                GUI.color = defaultColor;
            }
        }
    }

    private GameObject[] GetObjectsWithLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        List<GameObject> returnObjects = new List<GameObject>();
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach(GameObject gameObject in gameObjects)
        {
            if(gameObject.layer == layer)
            {
                returnObjects.Add(gameObject);
            }
        }
        return returnObjects.ToArray();
    }
}
