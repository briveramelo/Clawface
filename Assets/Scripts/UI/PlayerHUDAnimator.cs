using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDAnimator : MonoBehaviour
{
    Vector3 originalScale;


    private void Awake()
    {
        originalScale = transform.localScale;

        EventSystem.Instance.RegisterEvent(Strings.Events.COMBO_UPDATED, BounceCombo);
        EventSystem.Instance.RegisterEvent(Strings.Events.SCORE_UPDATED, BounceScore);
    }

    private void OnDestroy()
    {
        EventSystem.Instance.UnRegisterEvent(Strings.Events.COMBO_UPDATED, BounceCombo);
        EventSystem.Instance.UnRegisterEvent(Strings.Events.SCORE_UPDATED, BounceScore);
    }

    void BounceCombo (params object[] parameters)
    {

    }

    void BounceScore (params object[] parameters)
    {

    }
}
