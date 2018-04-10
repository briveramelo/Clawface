using UnityEngine;

[ExecuteInEditMode]
public abstract class AbstractAspectController<TBundle, TData> : MonoBehaviour
    where TBundle: AspectBundle<TData>
{
    #region Fields (Unity Serialization)

    #if UNITY_EDITOR

    [Header("Unity Editor Controls")]
    [SerializeField]
    private bool freezeUpdates = false;

    #endif

    [Header ("Aspect Controller Common")]
    [SerializeField]
    private TBundle[] bundles;

    #endregion

    #region Interface (Unity Lifecycle)

    private void Update()
    {
        #if UNITY_EDITOR

        if (freezeUpdates)
            return;

        #endif

        bool applied = false;
        foreach (TBundle bundle in bundles)
        {
            float aspect = bundle.GetAspectRatio();
            if (Mathf.Approximately(aspect, Camera.main.aspect))
            {
                ActOnBundle(bundle);
                applied = true;
                break;
            }
        }

        if (!applied)
        {
            DoDefaultAction();
        }
    }

    #endregion

    #region Interface (Protected)

    protected abstract void ActOnBundle(TBundle bundle);

    protected abstract void DoDefaultAction();

    #endregion
}
