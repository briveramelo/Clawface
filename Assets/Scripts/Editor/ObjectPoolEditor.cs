using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using ModMan;

[CustomEditor(typeof(ObjectPool))]
public class ObjectPoolEditor : Editor {

    SerializedProperty pools;
    List<Pool> poolsList;
    ReorderableList list;
    List<bool> prefabsFoldouts = new List<bool>();
    List<bool> poolFoldouts = new List<bool>();

    float LineHeight { get { return EditorGUIUtility.singleLineHeight; } }

    void OnEnable() {
        pools = serializedObject.FindProperty("pools");
        poolsList = (target as ObjectPool).pools;
        list = CreateList(serializedObject, pools);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    ReorderableList CreateList(SerializedObject obj, SerializedProperty prop) {
        ReorderableList list = new ReorderableList(obj, prop, true, true, true, true);

        
        list.drawHeaderCallback = rect => {
            EditorGUI.LabelField(rect, "Pools");
        };

        list.drawElementCallback = (rect, index, active, focused) => {

            Rect standardRect = new Rect(rect.position, new Vector2(rect.width, LineHeight)).AddPosition(new Vector2(5, 2.5f));
            EditorGUI.indentLevel++;
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            poolsList[index].name = poolsList[index].poolObjectType.ToString();
            poolFoldouts.AddUntil(index);
            poolFoldouts[index] = EditorGUI.Foldout(standardRect, poolFoldouts[index], poolsList[index].name);
            if (poolFoldouts[index]) {
                EditorGUI.indentLevel++;
                SerializedProperty prefabsProp = element.FindPropertyRelative("prefabs");

                poolsList[index].poolObjectType = (PoolObjectType)EditorGUI.EnumPopup(standardRect.AddPosition(Vector2.up * LineHeight*1), "Pool Object Type", poolsList[index].poolObjectType);
                poolsList[index].size = EditorGUI.IntField(standardRect.AddPosition(Vector2.up * LineHeight*2), "Pool Size", poolsList[index].size);
                prefabsFoldouts.AddUntil(index);
                prefabsFoldouts[index] = EditorGUI.PropertyField(standardRect.AddPosition(Vector2.up * LineHeight*3), prefabsProp, new GUIContent("Prefabs"), false);
                if (prefabsFoldouts[index]) {
                    EditorGUI.indentLevel++;
                    prefabsProp.arraySize = EditorGUI.IntField(standardRect.AddPosition(Vector2.up * LineHeight * 4), "Size", prefabsProp.arraySize);
                    for (int i = 0; i < prefabsProp.arraySize; i++) {
                        EditorGUI.PropertyField(standardRect.AddPosition(Vector2.up * LineHeight * (5+i)), prefabsProp.GetArrayElementAtIndex(i));
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        };

        list.elementHeightCallback = (index) => {
            Repaint();
            float height = LineHeight + 5;
            poolFoldouts.AddUntil(index);
            prefabsFoldouts.AddUntil(index);
            if (poolFoldouts[index]) {
                int lineCount = 3;
                height += lineCount * LineHeight;
                if (prefabsFoldouts[index]) {
                    int prefabCount = poolsList[index].prefabs.Count;
                    height += (prefabCount+1) * LineHeight;
                }
            }

            return height;
        };        

        list.onAddDropdownCallback = (rect, li) => {
            serializedObject.Update();
            li.serializedProperty.arraySize++;
            prefabsFoldouts.AddUntil(li.serializedProperty.arraySize);
            serializedObject.ApplyModifiedProperties();
        };        
        
        list.onRemoveCallback= (li) => {
            serializedObject.Update();
            li.serializedProperty.DeleteArrayElementAtIndex(list.index);
            serializedObject.ApplyModifiedProperties();
        };

        return list;
    }

}
