// IntRange.cs
// Author: Lai, Brandon, Aaron

using UnityEngine;

public class IntRangeAttribute : PropertyAttribute
{

    public int min;
    public int max;

    public IntRangeAttribute(int min, int max) {
        this.min = min;
        this.max = max;
    }
}

[System.Serializable]
public class IntRange
{

    [SerializeField] int min = 0;
    [SerializeField] int max = 10;

    public IntRange(int min, int max) {
        this.min = min;
        this.max = max;
    }
    
    public int Diff { get { return Max - Min; } }
    public int Min { get { return min; } set { min = value; } }
    public int Max { get { return max; } set { max = value; } }
    
    public int GetRandomValue() {
        return Random.Range(min, max);
    }    
}
