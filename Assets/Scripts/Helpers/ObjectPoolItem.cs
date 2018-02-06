using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolItem : MonoBehaviour {

    Transform myParent;
    public void SetParent(Transform newParent) {
        myParent = newParent;
    }

    void OnDisable() {
        transform.SetParent(myParent);
    }
}
