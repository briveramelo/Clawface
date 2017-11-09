using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ClawAnimationHandler : MonoBehaviour {

    #region private fields
    private Animator animator;
    private float animatorSpeed;
    #endregion

    #region Unity lifecycle
    private void Awake()
    {
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator);
        animatorSpeed = animator.speed;
    }
    #endregion

    #region Public functions
    public void StartAnimation()
    {
        animator.SetBool(Strings.ANIMATIONSTATE, true);
    }

    public void PauseAnimation()
    {
        animator.speed = 0;
    }

    public void FinishAnimation()
    {
        animator.speed = animatorSpeed;
    }
    #endregion
}
