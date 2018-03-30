using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefsToggle : MonoBehaviour {

    #region Serialized Unity Fields

    [SerializeField] private GameObject refsPane;

    #endregion

    #region Private Fields

    private bool isShowing = true;

    #endregion

    private void Start()
    {
        isShowing = refsPane.activeSelf;
    }

    public void TogglePaneAction()
    {

        isShowing = !isShowing;
        refsPane.SetActive(isShowing);

    }
}
