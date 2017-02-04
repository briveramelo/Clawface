/*
 Brandon Rivera-Melo
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour {

    [SerializeField] Stats playerStats;

    void OnTriggerStay(Collider col) {
        if (col.tag == Strings.CODEX) {
            CollectCodexEntry(col);
        }
        if (col.tag == Strings.ENEMY) {
            if ((Input.GetButton(Strings.PREPARETOPICKUPORDROP) &&
                Input.GetButtonDown(Strings.PREPARETOSWAP)) ||
                (Input.GetButtonDown(Strings.PREPARETOPICKUPORDROP) &&
                Input.GetButton(Strings.PREPARETOSWAP))) {

                if (col.GetComponent<ICollectable>().IsCollectable()) {
                    CollectSkin(col);
                }
            }
        }
    }

    void CollectCodexEntry(Collider col) {
        col.GetComponent<ICollectable>().Collect();

        CodexType codexType = col.GetComponent<ICodexLoggable>().GetCodexType();
        TheCodex.Instance.CollectCodex(codexType);
    }

    void CollectSkin(Collider col) {
        col.GetComponent<ICollectable>().Collect();

        playerStats.Modify(StatType.Health, (int)5);
    }

}
