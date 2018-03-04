using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollHandler : MonoBehaviour {

    #region private variables
    private bool isReady;
    private Vector3 startPosition;
    private Vector3 startScale;
    private Quaternion startRotation;
    #endregion

    #region unity lifecycle
    // Use this for initialization
    private void Awake () {
        if (startPosition == Vector3.zero)
        {
            startPosition = transform.localPosition;
            startScale = transform.localScale;
            startRotation = transform.localRotation;
            isReady = true;
        }
        CharacterJoint joint = GetComponent<CharacterJoint>();
        if (joint)
        {
            joint.enableProjection = true;
        }
    }
    #endregion

    #region public functions
    public void ResetBone()
    {
        if (isReady)
        {
            transform.localPosition = startPosition;
            transform.localScale = startScale;
            transform.localRotation = startRotation;
        }
    }
    #endregion

}
