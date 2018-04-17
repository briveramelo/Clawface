using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class EventSubscriber : RoutineRunner {

    protected virtual LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected virtual Dictionary<string, UnityAction<object[]>> EventSubscriptions { get { return new Dictionary<string, UnityAction<object[]>>() { }; } }

    protected virtual void Awake() {
        if (SubscriptionLifecycle==LifeCycle.AwakeDestroy) {
            RegisterEvents();
        }
    }
    protected virtual void Start() {
        if (SubscriptionLifecycle == LifeCycle.StartDestroy) {
            RegisterEvents();
        }
    }
    protected virtual void OnEnable() {
        if (SubscriptionLifecycle == LifeCycle.EnableDisable) {
            RegisterEvents();
        }
    }
    protected override void OnDisable() {
        if (SubscriptionLifecycle == LifeCycle.EnableDisable && EventSystem.Instance) {
            UnRegisterEvents();
        }
        base.OnDisable();
    }
    protected virtual void OnDestroy() {
        if ((SubscriptionLifecycle == LifeCycle.AwakeDestroy || SubscriptionLifecycle == LifeCycle.StartDestroy) && EventSystem.Instance) {
            UnRegisterEvents();
        }
    }

    void RegisterEvents() {
        Dictionary<string, UnityAction<object[]>> dic = EventSubscriptions;
        foreach (KeyValuePair<string, UnityAction<object[]>> eventSubscription in dic) {
            EventSystem.Instance.RegisterEvent(eventSubscription.Key, eventSubscription.Value);
        }
    }

    void UnRegisterEvents() {
        Dictionary<string, UnityAction<object[]>> dic = EventSubscriptions;
        foreach (KeyValuePair<string, UnityAction<object[]>> eventSubscription in dic) {
            EventSystem.Instance.UnRegisterEvent(eventSubscription.Key, eventSubscription.Value);
        }
    }

}

public enum LifeCycle {
    AwakeDestroy,
    StartDestroy,
    EnableDisable
}
