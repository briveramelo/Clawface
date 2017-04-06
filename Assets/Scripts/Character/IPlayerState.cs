
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState {

    #region Unity Lifecycle
    void Init(ref PlayerStateManager.StateVariables stateVariables);
    void StateUpdate();
    void StateFixedUpdate();
    #endregion
}
