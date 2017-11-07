using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollHandler : MonoBehaviour {

    #region private variables
    private bool isReady;
    private Vector3 startPosition;
    #endregion

    #region unity lifecycle
    // Use this for initialization
    private void Awake () {
        if (startPosition == Vector3.zero)
        {
            startPosition = transform.localPosition;
            isReady = true;
        }
    }
    #endregion

    #region public functions
    public void ResetBone()
    {
        if (isReady)
        {
            transform.localPosition = startPosition;
        }
    }
    #endregion

}
