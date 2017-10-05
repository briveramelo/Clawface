using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Float3
{
    public float x, y, z;

    public Float3 (float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 ToVector3 ()
    {
        return new Vector3 (x, y, z);
    }

    public static implicit operator Vector3(Float3 f)
    {
        return f.ToVector3();
    }
}
