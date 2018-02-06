using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IntRangeAttribute : PropertyAttribute {

    public int min;
    public int max;

    public IntRangeAttribute(int min, int max) {
        this.min = min;
        this.max = max;
    }
    public IntRangeAttribute() { }
}

public class FixedIntRangeAttribute : IntRangeAttribute {

    public FixedIntRangeAttribute(int min, int max) : base(min, max) { }
}

public class EditableIntRangeAttribute : FloatRangeAttribute {

    public EditableIntRangeAttribute() { }
}


[System.Serializable]
public class IntRange {

    [HideInInspector] public int minLimit, maxLimit;
    [SerializeField] int min = 0;
    [SerializeField] int max = 10;

    public IntRange(int min, int max) {
        this.min = min;
        this.max = max;
    }

    public int Diff { get { return max - min; } }
    public int Range { get { return maxLimit - minLimit; } }
    public int Min { get { return min; } set { min = value; } }
    public int Max { get { return max; } set { max = value; } }

    public int GetRandomValue() {
        return Random.Range(min, max+1);
    }
}
