using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {

	static SingletonMonoBehaviour<T> _Instance;

    public static SingletonEvent onSingletonInitialized = new SingletonEvent();

    protected void OnEnable () {
        if (_Instance == null) Awake(); 
    }

    protected void Awake() {
        if (_Instance != null)
            Debug.LogError ("An instance of " + typeof(T).ToString() + " already exists!");

        _Instance = this;
        onSingletonInitialized.Invoke();
    } 

    public static T Instance {
        get { return _Instance as T; }
    }

    public class SingletonEvent : UnityEvent { }
}
