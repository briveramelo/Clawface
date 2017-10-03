using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class PlayerAnimationEventsListener : MonoBehaviour {

    #region Private fields
    //private static UnityEvent faceOpenEvent;
    #endregion

    #region Public functions
    //public static UnityEvent FaceOpenEvent
    //{
    //    get
    //    {
    //        if(faceOpenEvent == null)
    //        {
    //            faceOpenEvent = new UnityEvent();
    //        }
    //        return faceOpenEvent;
    //    }
    //}

    public void FaceOpen()
    {
        //if (FaceOpenEvent != null)
        //{
        //    FaceOpenEvent.Invoke();
        //}
        object[] parameters = {};
        Assert.IsTrue(EventSystem.Instance.TriggerEvent("FaceOpen", parameters));
    }
    #endregion
}
