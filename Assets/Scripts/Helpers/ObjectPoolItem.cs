using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolItem : MonoBehaviour {

    Transform myParent;
    public void SetParent(Transform newParent) {
        myParent = newParent;
    }

    private void OnEnable() {
        if (myParent!=null) {
            transform.SetParent(myParent);
        }
    }    
}
