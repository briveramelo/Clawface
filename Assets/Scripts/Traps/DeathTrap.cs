using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrap : MonoBehaviour {

    [SerializeField] Transform respawnPoint;

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == Strings.Tags.PLAYER) {
            col.transform.position = respawnPoint.position;
        }
    }
}
