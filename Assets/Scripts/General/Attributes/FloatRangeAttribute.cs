using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FloatRangeAttribute : PropertyAttribute {

    public float min = 0;
    public float max = 1;

    public FloatRangeAttribute(float min, float max) {
        this.min = min;
        this.max = max;
    }
    public FloatRangeAttribute() { }
}

public class FixedFloatRangeAttribute : FloatRangeAttribute {

    public FixedFloatRangeAttribute(float min, float max) : base(min, max) { }
}

public class EditableFloatRangeAttribute : FloatRangeAttribute {

    public EditableFloatRangeAttribute() { }
}

