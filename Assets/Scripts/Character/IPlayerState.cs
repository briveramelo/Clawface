
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlayerState:MonoBehaviour {

    #region
    [SerializeField]
    protected int totalAttackPoses;
    [SerializeField]
    protected int[] highlightPoses;
    [SerializeField]
    protected int specialAttackPose;
    [SerializeField]
    protected int coolDownFrameCount;
    [SerializeField]
    protected int inputCheckFrameCount;
    protected PlayerStateManager.StateVariables stateVariables;
    #endregion

    #region Public Fields
    #endregion

    #region Unity Lifecycle
    public abstract void Init(ref PlayerStateManager.StateVariables stateVariables);
    public abstract void StateUpdate();
    public abstract void StateFixedUpdate();
    #endregion

    #region Public Methods
    public abstract void Attack();
    public abstract void SecondaryAttack(bool isHeld,float holdTime);
    #endregion

    #region Private Methods
    protected abstract void ResetState();
    #endregion
}