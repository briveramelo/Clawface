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
    private Color iconColor;
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    public void ShowAction(ActionType i_action)
    {
        if (i_action == ActionType.Skin)
        {
            actionImage.sprite = skinActionIcon;
            SetAlphaOfActionIcon(0f);

        }
        FadeInIconActionImage();
    }
    
    private void SetAlphaOfActionIcon(float i_val)
    {
        Color c = actionImage.color;
        c.a = i_val;
        actionImage.color = c;
    }

    private void FadeInIconActionImage()
    {
        LeanTween.value(gameObject, .25f, 1f, 1f).setOnUpdate((float val) =>
        {
            SetAlphaOfActionIcon(val);
        }).setOnComplete(FadeOut);
    }

    private void FadeOut()
    {
        LeanTween.value(gameObject, 1f, .25f, 1f).setOnUpdate((float val) =>
        {
            SetAlphaOfActionIcon(val);
        }).setOnComplete(FadeInIconActionImage);
    }
    #endregion

    #region Private Structures
    #endregion

}
