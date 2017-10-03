using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClawAnimationEventsListener : MonoBehaviour {

    #region Private fields
    private static UnityEvent clawArmExtendedEvent;
    #endregion

    #region Public functions
    public static UnityEvent ClawArmExtendedEvent
    {
        get
        {
            if(clawArmExtendedEvent == null)
            {
                clawArmExtendedEvent = new UnityEvent();
            }
            return clawArmExtendedEvent;
        }
    }

    public void ArmExtended()
    {
        if (ClawArmExtendedEvent != null)
        {
            ClawArmExtendedEvent.Invoke();
        }
    }
    #endregion
}
