using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public sealed class GridAspectResizer : MonoBehaviour {

    #region Fields (Unity Serialization)

    [SerializeField]
    private GridLayoutGroup layout;

    [SerializeField]
    private AspectBundle[] bundles;

    [SerializeField]
    private Vector2 defaultCellSize = new Vector2(250, 40);

    [SerializeField]
    private RectOffset defaultPadding = new RectOffset();

    #if UNITY_EDITOR
    [SerializeField]
    private bool freezeUpdates = false;
    #endif

    #endregion

    #region Interface (Unity Lifecycle)

    private void Update()
    {
        #if UNITY_EDITOR

        if (freezeUpdates)
            return;

        #endif

        bool applied = false;
        foreach (AspectBundle bundle in bundles)
        {
            float aspect = GetAspect(bundle.aspect);
            if (Mathf.Approximately(aspect, Camera.main.aspect))
            {
                layout.cellSize = bundle.cellSize;
                layout.padding = bundle.padding;
                applied = true;
            }
        }

        if (!applied)
        {
            layout.cellSize = defaultCellSize;
            layout.padding = defaultPadding;
        }
    }

    #endregion

    #region Interface (Private)

    private float GetAspect(Aspect aspect)
    {
        switch (aspect)
        {
            case Aspect.Aspect5By4:
                return 5F / 4F;
            case Aspect.Aspect4By3:
                return 4F / 3F;
            case Aspect.Aspect3By2:
                return 3F / 2F;
            case Aspect.Aspect16By10:
                return 16F / 10F;
            case Aspect.Aspect16By9:
                return 16F / 9F;
            default:
                throw new Exception("Provided aspect not listed!");
        }
    }

    #endregion

    #region Types (Public)

    public enum Aspect: int
    {
        Aspect5By4,
        Aspect4By3,
        Aspect3By2,
        Aspect16By10,
        Aspect16By9,
    }

    [Serializable]
    public struct AspectBundle
    {
        #region Fields (Public)

        public Aspect aspect;
        public Vector2 cellSize;
        public RectOffset padding;

        #endregion
    }

    #endregion
}
