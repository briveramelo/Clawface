using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mod : MonoBehaviour {

    ModSpot spot;
    ModType type;    

    public abstract void Activate();

    public abstract void DeActivate();
}
