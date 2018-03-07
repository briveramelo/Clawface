using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLEProp : PLEItem {
    public GameObject registeredProp;
    protected override string ColorTint { get { return "_AlbedoTint"; } }
}
