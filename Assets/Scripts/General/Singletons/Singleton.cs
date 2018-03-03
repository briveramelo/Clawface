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
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            return instance;
        }
    }
    #endregion

    #region Protected Fields

    [SerializeField] protected bool shouldRegister = true;
    [SerializeField] protected bool dontDestroyOnLoad = true;
    #endregion

    #region Private Fields
    private static T instance;
    private static bool applicationIsQuitting = false;
    private static bool guard = false;
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
            guard = true;
            Destroy(gameObject);
            Debug.LogWarning("Destroying duplicate singleton " + typeof(T) +"!");
        }
    }
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public virtual void OnDestroy()
    {
        if (!guard) {
            applicationIsQuitting = true;
        } else
        {
            guard = false;
        }
    }
    #endregion
}