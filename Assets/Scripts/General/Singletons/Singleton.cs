using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    #region Public Fields
    public static T Instance
    {
        get
        {            
            return instance;
        }
    }
    #endregion

    #region Protected Fields

    [SerializeField] protected bool shouldRegister = true;
    [SerializeField] protected bool dontDestroyOnLoad = true;
    #endregion

    #region Private Fields
    protected static T instance;
    #endregion

    #region Unity Lifecycle Functions
    protected virtual void Awake() {
        if (instance == null)
        {
            instance = this as T;
            if (Application.isPlaying)
            {
                if (shouldRegister) {
                    ServiceWrangler.Instance.RegisterSingleton(instance);
                }

                if (dontDestroyOnLoad) {
                    DontDestroyOnLoad(gameObject);
                }
            }  
        }
        else {
            if (!dontDestroyOnLoad) {
                instance = null;
            }
            Destroy(gameObject);
            Debug.LogWarning("Destroying duplicate singleton " + typeof(T) +"!");
        }
    }    
    #endregion
}