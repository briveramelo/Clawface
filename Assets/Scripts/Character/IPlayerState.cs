
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlayerState:RoutineRunner {

    #region Serialized Fields    
    [SerializeField]
    protected bool isBlockingState;
    #endregion

    #region Protected Fields
    protected PlayerStateManager.StateVariables stateVariables;
    #endregion

    #region Public Fields
    #endregion

    #region Unity Lifecycle
    public abstract void Init(ref PlayerStateManager.StateVariables stateVariables);
    public abstract void StateUpdate();
    public abstract void StateFixedUpdate();
    public abstract void StateLateUpdate();
    #endregion

    #region Public Methods
    public bool IsBlockingState()
    {
        return isBlockingState;
    }
    #endregion

    #region Private Methods
    protected abstract void ResetState();
    #endregion
}