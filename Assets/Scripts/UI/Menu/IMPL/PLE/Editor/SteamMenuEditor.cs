using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SteamMenu))]
public class SteamMenuEditor : Editor {

    SteamMenuSubMenu selectedMenu;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        selectedMenu = (SteamMenuSubMenu)EditorGUILayout.EnumPopup("Show Menu", selectedMenu);
        (target as SteamMenu).ShowMenu(selectedMenu);
    }
}


