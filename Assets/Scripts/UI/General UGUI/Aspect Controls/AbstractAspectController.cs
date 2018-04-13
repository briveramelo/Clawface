using UnityEngine;

[ExecuteInEditMode]
public abstract class AbstractAspectController<TBundle, TData> : MonoBehaviour
    where TBundle: AspectBundle<TData>
{
    #region Fields (Unity Serialization)

    [Header("Unity Editor Controls")]
    [SerializeField]
    private bool freezeUpdates = false;

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
            if (Mathf.Approximately(aspect, GetCompareAspect()))
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

    #region Interface (Private)

    private float GetCompareAspect()
    {
        #if UNITY_EDITOR
        return Camera.main.aspect;
        #else
        Resolution resolution = SettingsManager.Instance.Resolution;
        return 1F * resolution.width / resolution.height;
        #endif
    }

    #endregion
}
