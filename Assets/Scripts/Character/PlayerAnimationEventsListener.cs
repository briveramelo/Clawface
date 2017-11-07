using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class PlayerAnimationEventsListener : MonoBehaviour {

    #region Public functions
    public void FaceOpen()
    {
        EventSystem.Instance.TriggerEvent(Strings.Events.FACE_OPEN);
    }
    #endregion
}
