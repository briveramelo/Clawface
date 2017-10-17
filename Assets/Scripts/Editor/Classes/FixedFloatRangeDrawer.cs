using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FixedFloatRangeAttribute))]
public class FixedFloatRangeDrawer : FloatRangeDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        FixedFloatRangeAttribute attrib = attribute as FixedFloatRangeAttribute;
        attribMin = attrib.min;
        attribMax = attrib.max;
        base.OnGUI(position, property, label);
    }
}
