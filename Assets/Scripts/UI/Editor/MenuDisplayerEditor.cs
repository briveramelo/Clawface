using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MenuDisplayer))]
public class MenuDisplayerEditor : Editor {

    SerializedProperty displayedMenu;
    private void OnEnable() {
        displayedMenu = serializedObject.FindProperty("displayedMenu");
    }
    public override void OnInspectorGUI() {
        serializedObject.Update();        
        displayedMenu.enumValueIndex = (int)((Menu.MenuType)EditorGUILayout.EnumPopup("Show Menu", (Menu.MenuType)(displayedMenu.enumValueIndex)));
        (target as MenuDisplayer).ShowMenuExclusive((Menu.MenuType)displayedMenu.enumValueIndex);
        serializedObject.ApplyModifiedProperties();
    }
}