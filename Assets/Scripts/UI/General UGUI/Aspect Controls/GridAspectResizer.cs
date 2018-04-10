using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public sealed class GridAspectResizer : AbstractAspectController<GridAspectResizer.GridAspectBundle,
    GridAspectResizer.GridAspectBundleData> {

    #region Fields (Unity Serialization)
    
    [Header("GridAspectResizer Only")]
    [SerializeField]
    private GridLayoutGroup layout;

    [SerializeField]
    private GridAspectBundleData defaultData = new GridAspectBundleData
    {
        cellSize = new Vector2(250, 40),
        padding = new RectOffset()
    };

    #endregion

    #region Interface (AbstractAspectController<GridAspectBundle, GridAspectBundleData>)

    protected override void ActOnBundle(GridAspectBundle bundle)
    {
        layout.cellSize = bundle.data.cellSize;
        layout.padding = bundle.data.padding;
    }

    protected override void DoDefaultAction()
    {
        layout.cellSize = defaultData.cellSize;
        layout.padding = defaultData.padding;
    }

    #endregion

    #region Types (Public)

    [Serializable]
    public struct GridAspectBundleData
    {
        #region Fields (Public)

        public Vector2 cellSize;

        public RectOffset padding;

        #endregion
    }

    [Serializable]
    public class GridAspectBundle : AspectBundle<GridAspectBundleData>
    {
        // Nothing to do here
    }

    #endregion
}
