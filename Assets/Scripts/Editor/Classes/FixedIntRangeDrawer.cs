using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FixedIntRangeAttribute))]
public class FixedIntRangeDrawer : IntRangeDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        FixedIntRangeAttribute attrib = attribute as FixedIntRangeAttribute;
        attribMin = attrib.min;
        attribMax = attrib.max;
        base.OnGUI(position, property, label);
    }
}
