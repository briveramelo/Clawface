using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//sends "attribMin/attribMax to the base class so that the limits of the range can be edited in real time
[CustomPropertyDrawer(typeof(EditableFloatRangeAttribute))]
public class EditableFloatRangeDrawer : FloatRangeDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        attribMin = property.FindPropertyRelative("minLimit").floatValue;
        attribMax = property.FindPropertyRelative("maxLimit").floatValue;
        base.OnGUI(position, property, label);
    }
}
