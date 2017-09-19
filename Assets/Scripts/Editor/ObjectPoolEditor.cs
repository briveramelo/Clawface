using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;

[CustomEditor(typeof(ObjectPool))]
public class ObjectPoolEditor : Editor {

    SerializedProperty pools;
    //List<Pool> poolsList;
    ReorderableList list;

    void OnEnable() {
        pools = serializedObject.FindProperty("pools");
        //poolsList = (target as ObjectPool).pools;
        list = CreateList(serializedObject, pools);

        //list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
        //    var element = pools.GetArrayElementAtIndex(index);
        //    rect.y += 2;
            
        //    EditorGUI.PropertyField(new Rect(rect.x, rect.y, 500, EditorGUIUtility.singleLineHeight), element, true);
        //    //EditorGUI.PropertyField( new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Prefab"), GUIContent.none);
        //    //EditorGUI.PropertyField( new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Count"), GUIContent.none);
        //};
    }

    public override void OnInspectorGUI() {
        
        serializedObject.Update();
        list.DoLayoutList();
        //list.DoList(new Rect(new Vector2(), new Vector2(400, 900)));
        serializedObject.ApplyModifiedProperties();
    }

    ReorderableList CreateList(SerializedObject obj, SerializedProperty prop) {
        ReorderableList list = new ReorderableList(obj, prop, true, true, true, true);

        list.drawHeaderCallback = rect => {
            EditorGUI.LabelField(rect, "Pools");
        };

        List<float> heights = new List<float>(prop.arraySize);
        List<bool> foldOuts = new List<bool>(prop.arraySize);
        for (int i = 0; i < prop.arraySize; i++) {
            foldOuts.Add(false);
        }


        list.drawElementCallback = (rect, index, active, focused) => {
            
            float height = EditorGUIUtility.singleLineHeight * (foldOuts[index] ? 1.25f : 5);

            TryAssignHeights(ref heights, prop, index, height);
            TryAssignFoldouts(ref foldOuts, prop, index, foldOuts[index]);

            float margin = height / 10;
            rect.y += margin;
            rect.height = (height / 5) * 4;
            rect.width = rect.width / 2 - margin / 2;

            
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            ///Sprite s = (element.objectReferenceValue as Sprite);
            element.FindPropertyRelative("name").stringValue = ((PoolObjectType)(element.FindPropertyRelative("poolObjectType").enumValueIndex)).ToString();
            foldOuts[index] = EditorGUI.PropertyField(rect, element, true);
            rect.x += margin;
            //EditorGUI.ObjectField(rect, element, GUIContent.none);
        };

        list.elementHeightCallback = (index) => {
            Repaint();
            float height = 0;
            bool foldout = false;

            TryAssignHeights(ref heights, prop, index, height);
            TryAssignFoldouts(ref foldOuts, prop, index, foldout);

            return height;
        };

        list.drawElementBackgroundCallback = (rect, index, active, focused) => {
            rect.height = heights[index];
            //Texture2D tex = new Texture2D(1, 1);
            //tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
            //tex.Apply();
            //if (active)
            //    GUI.DrawTexture(rect, tex as Texture);
        };

        list.onAddDropdownCallback = (rect, li) => {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Element"), false, () => {
                serializedObject.Update();
                li.serializedProperty.arraySize++;
                serializedObject.ApplyModifiedProperties();
            });

            menu.ShowAsContext();

            ResizeHeights(ref heights, prop);
            ResizeFoldouts(ref foldOuts, prop);
        };

        return list;
    }

    void TryAssignHeights(ref List<float> heights, SerializedProperty prop, int index, float height) {
        try {
            heights[index] = height;
        }
        catch (ArgumentOutOfRangeException e) {
            Debug.LogWarning(e.Message);
        }
        finally {
            ResizeHeights(ref heights, prop);
        }
    }

    void TryAssignFoldouts(ref List<bool> foldOuts, SerializedProperty prop, int index, bool foldout) {
        try {
            foldOuts[index] = foldout;
        }
        catch (ArgumentOutOfRangeException e) {
            Debug.LogWarning(e.Message);
        }
        finally {
            ResizeFoldouts(ref foldOuts, prop);
        }
    }

    void ResizeHeights(ref List<float> heights, SerializedProperty prop) {
        float[] floats = heights.ToArray();
        Array.Resize(ref floats, prop.arraySize);
        heights = floats.ToList();
    }

    void ResizeFoldouts(ref List<bool> foldOuts, SerializedProperty prop) {
        bool[] floats = foldOuts.ToArray();
        Array.Resize(ref floats, prop.arraySize);
        foldOuts = floats.ToList();
    }

}
