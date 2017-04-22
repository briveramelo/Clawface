using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFXEffects : MonoBehaviour {

    public void PlayButtonConfirm()
    {
        SFXManager.Instance.Play(SFXType.UI_Click,gameObject.transform.position);
    }

    public void PlayButtonSelect()
    {
        SFXManager.Instance.Play(SFXType.UI_Hover, gameObject.transform.position);
    }

    public void PlayButtonDeny()
    {
        SFXManager.Instance.Play(SFXType.UI_Back, gameObject.transform.position);
    }
}
