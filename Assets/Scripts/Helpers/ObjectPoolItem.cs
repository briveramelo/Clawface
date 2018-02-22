using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolItem : MonoBehaviour {

    Transform myParent;
    public void SetParent(Transform newParent) {
        myParent = newParent;
    }
    public void ResetParent() {
        transform.SetParent(myParent);
    }

    private void OnEnable() {
        if (myParent!=null) {
            ResetParent();
        }
    }    
}
