using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CapsuleBounds {

    public Transform start, end;
    public float radius;
}

[System.Serializable]
public class BoxBounds {

    public Transform center, corner;
    public Vector3 Center{ get{ return center.position;} }
    public Vector3 Corner{ get{ return corner.position;} }
    public Vector3 Size{ get{ return 2*(Center-Corner);} }
}
