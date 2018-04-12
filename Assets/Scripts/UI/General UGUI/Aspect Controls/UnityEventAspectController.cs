using System;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public sealed class UnityEventAspectController :
    AbstractAspectController<UnityEventAspectController.UnityEventAspectBundle, UnityEvent>
{
    #region Fields (Unity Serialization)

    [Header("UnityEventAspectController")]
    [SerializeField]
    private UnityEvent defaultEvent;

    #endregion

    #region Interface (AbstractAspectController<UnityEventAspectController.UnityEventAspectBundle, UnityEvent>)

    protected override void ActOnBundle(UnityEventAspectBundle bundle)
    {
        if (bundle.data != null)
        {
            bundle.data.Invoke();
        }
    }

    protected override void DoDefaultAction()
    {
        if (defaultEvent != null)
        {
            defaultEvent.Invoke();
        }
    }

    #endregion

    #region Types (Public)

    [Serializable]
    public class UnityEventAspectBundle : AspectBundle<UnityEvent>
    {
        // Nothing to do here
    }

    #endregion
}
