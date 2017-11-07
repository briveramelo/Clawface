using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ClawAnimationHandler : MonoBehaviour {

    #region Serialized fields
    [SerializeField]
    private Transform clawPalm;
    #endregion

    #region private fields
    private Animator animator;
    private GameObject target;
    #endregion

    #region Unity lifecycle
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    #endregion

    #region Public functions
    public void ArmExtended()
    {
        EventSystem.Instance.TriggerEvent(Strings.Events.ARM_EXTENDED, clawPalm);
    }

    public void AnimationCompleted()
    {
        EventSystem.Instance.TriggerEvent(Strings.Events.ARM_ANIMATION_COMPLETE);
    }
    #endregion

    #region private functions    
    #endregion
}
