﻿// SingletonMonoBehaviour.cs

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Editor-friendly singleton base class.
/// </summary>
[ExecuteInEditMode]
public abstract class SingletonMonoBehaviour<T> : 
    MonoBehaviour where T : MonoBehaviour 
{
    #region Vars

    /// <summary>
    /// The currently active instance of this class.
    /// </summary>
    static SingletonMonoBehaviour<T> _Instance;

    /// <summary>
    /// Invoked when the instance of this class is initialized.
    /// </summary>
    public static UnityEvent OnSingletonInitializedEditor = new UnityEvent();

    #endregion
    #region Unity Callbacks

    protected void OnEnable () {
        if (_Instance == null) Awake(); 
    }

    protected void Awake() {
        if (_Instance != null)
            Debug.LogError ("An instance of " + typeof(T).ToString() + " already exists!");

        _Instance = this;
        if (!Application.isPlaying)
            OnSingletonInitializedEditor.Invoke();
    }

    #endregion
    #region Properties

    /// <summary>
    /// Returns the active instance of this Singleton (read-only).
    /// </summary>
    public static T Instance { get { return _Instance as T; } }

    #endregion
}
