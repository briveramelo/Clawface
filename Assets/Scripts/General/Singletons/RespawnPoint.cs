using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnPoint : Singleton<RespawnPoint> {

    protected RespawnPoint() { }

    void Start() {
        if (GameObject.Find("RespawnPoint")) {
        }
        else {
            Debug.LogWarning("Put a GameObject named 'RespawnPoint' in your scene!!!");
        }
        Destroy(gameObject);
    }
}
