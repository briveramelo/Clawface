using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatRange {
    [SerializeField] float _min = 1f;
    [SerializeField] float _max = 1f;

    public float Min {
        get { return _min; }
        set { _min = value; }
    }

    public float Max {
        get { return _max; }
        set { _max = value; }
    }

    public float GetRandomValue () {
        return Random.Range (_min, _max);
    }
}
