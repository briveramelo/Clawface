using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlRefsPane : MonoBehaviour {

    #region Serialized Unity Fields

    [SerializeField] private GameObject helpClickText;

    [SerializeField] private GameObject[] detailPanes;


    [SerializeField] private RectTransform myRect;
    // floor 0
    // spawn 1
    // waves 2
    #endregion

    #region Private Fields

    bool isShowing = false;

    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.RightControl))
        {
            print(isShowing + " at " + Time.time);
            isShowing = !isShowing;

            if (!isShowing)
            {

                foreach (GameObject g in detailPanes)
                {
                    g.SetActive(false);
                }

                helpClickText.SetActive(true);

                myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);


            }
            else
            {
                foreach (GameObject g in detailPanes)
                {
                    g.SetActive(true);
                }

                helpClickText.SetActive(false);

                myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150);


            }
        }
    }

    #endregion
}
