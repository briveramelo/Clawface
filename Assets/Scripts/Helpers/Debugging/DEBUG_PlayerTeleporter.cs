using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_PlayerTeleporter : Singleton<DEBUG_PlayerTeleporter> {

    protected DEBUG_PlayerTeleporter() { }

	[SerializeField] Transform teleportTarget1, teleportTarget2;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.T)) {
            Teleport(teleportTarget1);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            Teleport(teleportTarget2);
        }
	}

    void Teleport(Transform target) {
        GameObject player = GameObject.FindGameObjectWithTag(Strings.Tags.PLAYER);
        if (player)
        {
            player.transform.position = target.position;
        }
    }
}
