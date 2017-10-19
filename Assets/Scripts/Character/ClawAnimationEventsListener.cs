using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ClawAnimationEventsListener : MonoBehaviour {

    #region Serialized fields
    [SerializeField]
    private Transform clawPalm;
    #endregion

    #region Public functions
    public void ArmExtended()
    {
        Assert.IsTrue(EventSystem.Instance.TriggerEvent(Strings.Events.ARM_EXTENDED, clawPalm));
    }

    public void AnimationCompleted()
    {
        Assert.IsTrue(EventSystem.Instance.TriggerEvent(Strings.Events.ARM_ANIMATION_COMPLETE));
    }
    #endregion
}
