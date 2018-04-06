using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RefsToggle : MonoBehaviour {

    #region Serialized Unity Fields

    [SerializeField] private GameObject refsPane;
    [SerializeField] private Selectable toggleButton;
    #endregion

    #region Private Fields

    private bool IsShowing {
        get { return refsPane.activeSelf; }
        set { refsPane.SetActive(value); }
    }
    private bool IsLocked {
        get { return !toggleButton.interactable; }
        set { toggleButton.interactable = !value; }
    }
    #endregion

    public void TryTogglePaneAction()
    {
        if (!IsLocked) {
            IsShowing = !IsShowing;
        }
    }
    public void HidePane() {
        IsShowing = false;
    }

    public void LockPane(bool isLocked) {
        IsLocked = isLocked;
    }
}
