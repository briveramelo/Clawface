using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventSubscriber : RoutineRunner {

    protected virtual LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected virtual Dictionary<string, FunctionPrototype> EventSubscriptions { get { return null; } }

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
    }
    protected virtual void OnDestroy() {
        if ((SubscriptionLifecycle == LifeCycle.AwakeDestroy || SubscriptionLifecycle == LifeCycle.StartDestroy) && EventSystem.Instance) {
            UnRegisterEvents();
        }
    }

    void RegisterEvents() {
        foreach (KeyValuePair<string, FunctionPrototype> eventSubscription in EventSubscriptions) {
            EventSystem.Instance.RegisterEvent(eventSubscription.Key, eventSubscription.Value);
        }
    }

    void UnRegisterEvents() {
        foreach (KeyValuePair<string, FunctionPrototype> eventSubscription in EventSubscriptions) {
            EventSystem.Instance.UnRegisterEvent(eventSubscription.Key, eventSubscription.Value);
        }
    }

}

public enum LifeCycle {
    AwakeDestroy,
    StartDestroy,
    EnableDisable
}
