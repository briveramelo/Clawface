using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModCanvas : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private Image modIconGraphic;
    #endregion

    private void OnEnable()
    {
        FadeInIconActionImage();
    }
    private void OnDisable()
    {
        FadeOutIconActionImage();
    }

    private void FadeOutIconActionImage()
    {
        Color c = modIconGraphic.color;
        c.a = 0f;
        modIconGraphic.color = c;
    }


    private void SetAlphaOfActionIcon(float i_val)
    {
        Color c = modIconGraphic.color;
        c.a = i_val;
        modIconGraphic.color = c;
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
}
