using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexEntry : MonoBehaviour, ICollectable {

    [SerializeField] CodexType codexType;
    [SerializeField] GameObject codexToPass;

    GameObject ICollectable.Collect()
    {
        TheCodex.Instance.CollectCodex(codexType);
        Destroy(gameObject, 0.01f);
        return codexToPass;
    }

}
