﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SpawnManager))]
public class SpawnManagerEditor : Editor
{
    SpawnManager manager;
    GameObject managerObject;


    private ReorderableList list;

    private void OnEnable()
    {
        manager = (SpawnManager)target;
        managerObject = manager.gameObject;

        list = new ReorderableList(serializedObject, serializedObject.FindProperty("spawners"), true, false, true, true);

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Prefab"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Time"), GUIContent.none);
        };


        list.onAddCallback = (ReorderableList l) => 
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);

            GameObject _prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Enemies/MallCopSpawner.prefab", typeof(GameObject)) as GameObject;

            GameObject _instance = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
            _instance.transform.SetParent(managerObject.transform);
            _instance.transform.localPosition = Vector3.zero;

            element.FindPropertyRelative("Prefab").objectReferenceValue = _instance;

        };

        list.onRemoveCallback = (ReorderableList l) => 
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the Spawner?", "Yes"))
            {
                var element = l.serializedProperty.GetArrayElementAtIndex(l.index);
                GameObject deletedObject = element.FindPropertyRelative("Prefab").objectReferenceValue as GameObject;

                if(deletedObject != null)
                    DestroyImmediate(deletedObject);

                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            }
        };


    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

}