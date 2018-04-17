using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Deathcam : EventSubscriber {

    [SerializeField]
    private Transform deathCam;
    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, UnityAction<object[]>> EventSubscriptions {
        get {
            return new Dictionary<string, UnityAction<object[]>>() {
            { Strings.Events.PLAYER_KILLED, Unparent},
        };
        }
    }
    #endregion

    public void Unparent(params object[] items)
    {
        deathCam.gameObject.SetActive(true);
    }
}
