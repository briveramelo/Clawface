using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinningState : MonoBehaviour, IPlayerState
{

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    #endregion

    #region Private Fields
    private PlayerStateManager.StateVariables stateVariables;
    #endregion

    #region Unity Lifecycle
    public void Init(ref PlayerStateManager.StateVariables stateVariables)
    {
        this.stateVariables = stateVariables;
    }

    public void StateFixedUpdate()
    {

    }

    public void StateUpdate()
    {
        stateVariables.currentEnemy.GetComponent<ISkinnable>().DeSkin();
        stateVariables.stateFinished = true;
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
