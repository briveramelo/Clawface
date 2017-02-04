using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexEntry : MonoBehaviour, ICollectable {

    [SerializeField] CodexType codexType;

    void ICollectable.Collect()
    {
        TheCodex.Instance.CollectCodex(codexType);
        Destroy(gameObject);
    }

    bool ICollectable.IsCollectable()
    {
        return true;
    }

}
