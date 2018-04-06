using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;

public class SwapCamerasOnLevelEnd : EventSubscriber {

    #region Serialized
    [SerializeField]
    private Transform endCamera;

    [SerializeField]
    private Transform keiraMesh;

    [SerializeField]
    private MoveState moveState;

    [SerializeField]
    private Rigidbody playerRigidbody;
    #endregion

    #region Unity Lifetime
    #endregion
    // Use this for initialization

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions { get {
        return new Dictionary<string, FunctionPrototype>() {
            { Strings.Events.LEVEL_COMPLETED, SwitchCameras},            
        };
    } }
    #endregion

    #region Private Methods
    void SwitchCameras(params object[] parameters)
    {
        endCamera.gameObject.SetActive(true);
        moveState.enabled = false;
        playerRigidbody.velocity = Vector3.zero;
        // keiraMesh.localRotation = Quaternion.Euler(0f, keiraMesh.transform.rotation.y, 0f);
    }
    #endregion
}
