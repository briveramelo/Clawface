using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class DeathcamSpin : EventSubscriber
{

    #region Serialized
    [SerializeField] private Transform deathCamera;
    #endregion

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, UnityAction<object[]>> EventSubscriptions {
        get {
            return new Dictionary<string, UnityAction<object[]>>() {
                { Strings.Events.PLAYER_KILLED, OnDeath },
            };
        }
    }
    #endregion

    #region Unity Lifecycle
    #endregion


    #region Public


    #endregion

    #region Private
    public void OnDeath(params object[] items)
    {
        deathCamera.gameObject.SetActive(true);
    }
    #endregion
}
