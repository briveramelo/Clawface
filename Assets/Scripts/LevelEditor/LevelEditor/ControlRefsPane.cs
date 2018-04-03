using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerLevelEditor;

public class ControlRefsPane : MonoBehaviour {

    #region Serialized Unity Fields

    [SerializeField] private GameObject[] detailPanes;
    // floor 0
    // spawn 1
    // waves 2
    [SerializeField] private MainPLEMenu mainPLE;
    [SerializeField] private Text titleBar;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        titleBar.text = "V HELP V";
        SetDisplayedContent(mainPLE.currentDisplayedMenu);
    }
    private void OnDisable()
    {
        titleBar.text = "^ HELP ^";
    }

    #endregion

    #region Private Fields
    public PLEMenu currentDisplayedMenu;

    #endregion

    #region Public Interface

    public void SetDisplayedContent(PLEMenu i_menu)
    {

        foreach (GameObject g in detailPanes)
        {
            g.SetActive(false);
        }

        switch(i_menu)
        {
            case PLEMenu.FLOOR:
                detailPanes[0].SetActive(true);
                break;
            case PLEMenu.SPAWN:
                detailPanes[1].SetActive(true);
                break;
            case PLEMenu.WAVE:
                detailPanes[2].SetActive(true);
                break;
        }

    }




    #endregion

}


