using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DisableOnPlayerDeath : EventSubscriber
{

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.AwakeDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
            { Strings.Events.PLAYER_KILLED, HandlePlayerDeath},
        };
        }
    }
    #endregion    

    void HandlePlayerDeath (params object[] parameters)
    {
        gameObject.SetActive(false);
    }
}
