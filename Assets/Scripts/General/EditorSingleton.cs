// EditorSingleton.cs
// Author: Aaron

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Editor-friendly singleton base class.
/// </summary>
[ExecuteInEditMode]
public abstract class EditorSingleton<T> : 
    Singleton<T> where T : MonoBehaviour 
{
    #region Public Fields

    /// <summary>
    /// Invoked when the instance of this class is initialized.
    /// </summary>
    public static UnityEvent OnSingletonInitializedEditor = new UnityEvent();

    #endregion
    #region Unity Lifecycle

    new protected void Awake() {
        base.Awake();

        if (!Application.isPlaying)
            OnSingletonInitializedEditor.Invoke();
    }

    #endregion
}
