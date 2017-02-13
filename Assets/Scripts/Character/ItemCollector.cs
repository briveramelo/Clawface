/*
 Brandon Rivera-Melo
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour {

    [SerializeField] Stats playerStats;

    void OnTriggerStay(Collider col) {
        if (col.tag == Strings.CODEXENTRY) {
            CollectCodexEntry(col);
        }
        if (col.tag == Strings.ENEMY) {
            if ((Input.GetButton(Strings.RIGHTBUMPER) &&
                Input.GetButtonDown(Strings.LEFTBUMPER)) ||
                (Input.GetButtonDown(Strings.RIGHTBUMPER) &&
                Input.GetButton(Strings.LEFTBUMPER))) {

                if (col.GetComponent<ISkinnable>().IsSkinnable()) {
                    CollectSkin(col);
                }
            }
        }
    }

    void CollectCodexEntry(Collider col) {
        col.GetComponent<ICollectable>().Collect();        
    }

    void CollectSkin(Collider col) {
        col.GetComponent<ISkinnable>().DeSkin();
        playerStats.Modify(StatType.Health, 5);
    }

}
