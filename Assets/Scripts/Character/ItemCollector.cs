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
            Collect(col);
        }
        if (col.tag == Strings.ENEMY) {
            if ((Input.GetButton(Strings.PREPARETOPICKUPORDROP) &&
                Input.GetButtonDown(Strings.PREPARETOSWAP)) ||
                (Input.GetButtonDown(Strings.PREPARETOPICKUPORDROP) &&
                Input.GetButton(Strings.PREPARETOSWAP))) {

                Collect(col);
            }
        }
    }

    void Collect(Collider collectable) {
        CollectableType collectableType = collectable.GetComponent<ICollectable>().Collect();

        switch (collectableType) {
            case CollectableType.Codex:
                CodexType codexType = collectable.GetComponent<ICodexLoggable>().GetCodexType();
                TheCodex.Instance.CollectCodex(codexType);
                break;
            case CollectableType.Skin:
                playerStats.Modify(StatType.Health, (int)5);
                break;
        }
    }
}
