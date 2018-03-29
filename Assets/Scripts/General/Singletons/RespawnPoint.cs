using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnPoint : Singleton<RespawnPoint> {

    protected RespawnPoint() { }

    protected override void Start() {
        base.Start();
        if (GameObject.Find(Strings.RESPAWN_POINT)) {
        }
        else {
            Debug.LogWarning(string.Format("Put a GameObject named '{0}' in your scene!!!", Strings.RESPAWN_POINT));
        }
        Destroy(gameObject);
    }
}
