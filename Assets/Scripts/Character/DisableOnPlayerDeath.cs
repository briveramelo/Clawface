using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DisableOnPlayerDeath : EventSubscriber
{

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.AwakeDestroy; } }
    protected override Dictionary<string, UnityAction<object[]>> EventSubscriptions {
        get {
            return new Dictionary<string, UnityAction<object[]>>() {
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
