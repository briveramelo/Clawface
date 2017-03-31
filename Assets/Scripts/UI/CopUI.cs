using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopUI : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private Sprite skinActionIcon;
    [SerializeField]
    private Image actionImage;
    #endregion

    #region Private Fields
    private Camera mainCamera;
    private Canvas copCanvas;


    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        //billboard to main camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void ShowAction(ActionType i_action)
    {
        if (i_action == ActionType.Skin)
        {
            actionImage.sprite = skinActionIcon;
        }
    }

    private void ClearACtion()
    {
        actionImage.sprite = null;
    }



    #endregion

    #region Private Structures
    #endregion

}
