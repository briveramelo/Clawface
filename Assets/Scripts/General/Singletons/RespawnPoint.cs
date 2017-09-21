using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : Singleton<RespawnPoint> {

    protected RespawnPoint() { }

    void Start() {
        if (GameObject.Find("RespawnPoint")) {
            Destroy(gameObject);
        }
        else {
            name = "RespawnPoint";
        }
    }
}
