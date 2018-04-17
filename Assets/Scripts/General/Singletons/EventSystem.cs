using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public delegate void ParamsPrototype (params object[] parameters);

public class ParamsEvent : UnityEvent<object[]> { }

public class EventSystem : Singleton<EventSystem> {

    #region Private variables
    private Dictionary<string, ParamsEvent> eventMap = new Dictionary<string, ParamsEvent>();
    #endregion

    #region Private function    
    #endregion

    #region Public functions
    public bool RegisterEvent(string key, UnityAction<object[]> listener)
    {
        if (eventMap.ContainsKey(key))
        {
            ParamsEvent paramsEvent;
            if (eventMap.TryGetValue(key, out paramsEvent))
            {
                //functionPrototype += eventFunction;
                paramsEvent.AddListener(listener);
                //eventMap[key] = functionPrototype;
            }
        }
        else
        {
            //ParamsEvent functionPrototype = eventFunction;
            ParamsEvent newEvent = new ParamsEvent();
            newEvent.AddListener(listener);
            eventMap.Add(key, newEvent);

        }
        return true;
    }

    public bool UnRegisterEvent(string key, UnityAction<object[]> listener)
    {
        if (eventMap.ContainsKey(key))
        {
            ParamsEvent paramsEvent;
            if (eventMap.TryGetValue(key, out paramsEvent))
            {

                //functionPrototype -= eventFunction;
                paramsEvent.RemoveListener(listener);
                if(paramsEvent != null)
                {
                    eventMap[key] = paramsEvent;
                }
                else
                {
                    eventMap.Remove(key);
                }
            }
        }
        return true;
    }

    public bool TriggerEvent(string key, params object[] parameters)
    {
        ParamsEvent paramsEvent;
        if (eventMap.TryGetValue(key, out paramsEvent))
        {
            paramsEvent.Invoke(parameters);
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

}
