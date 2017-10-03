using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : Singleton<EventSystem> {

    #region Private variables
    public delegate void FunctionPrototype(object[] parameters);
    private Dictionary<string, FunctionPrototype> eventMap = new Dictionary<string, FunctionPrototype>();
    #endregion

    #region Private function    
    #endregion

    #region Public functions
    public bool RegisterEvent(string key, FunctionPrototype eventFunction)
    {
        if (eventMap.ContainsKey(key))
        {
            FunctionPrototype functionPrototype;
            if (eventMap.TryGetValue(key, out functionPrototype))
            {
                functionPrototype += eventFunction;
                eventMap[key] = functionPrototype;
            }
        }
        else
        {
            FunctionPrototype functionPrototype = eventFunction;
            eventMap.Add(key, functionPrototype);
        }
        return true;
    }

    public bool UnRegisterEvent(string key, FunctionPrototype eventFunction)
    {
        if (eventMap.ContainsKey(key))
        {
            FunctionPrototype functionPrototype;
            if (eventMap.TryGetValue(key, out functionPrototype))
            {
                functionPrototype -= eventFunction;
                if(functionPrototype != null)
                {
                    eventMap[key] = functionPrototype;
                }
                else
                {
                    eventMap.Remove(key);
                }
            }
        }
        return true;
    }

    public bool TriggerEvent(string key, object[] parameters)
    {
        FunctionPrototype functionPrototype;
        if (eventMap.TryGetValue(key, out functionPrototype))
        {
            functionPrototype(parameters);
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

}
