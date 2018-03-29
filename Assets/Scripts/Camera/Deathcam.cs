using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathcam : EventSubscriber {

    [SerializeField]
    private Transform deathCam;
    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
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
