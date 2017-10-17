using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EditableIntRangeAttribute))]
public class EditableIntRangeDrawer : IntRangeDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        attribMin = property.FindPropertyRelative("minLimit").intValue;
        attribMax = property.FindPropertyRelative("maxLimit").intValue;
        base.OnGUI(position, property, label);
    }
}
