// ObjectData.cs
// Author: Aaron

using System;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Serializable class to hold object information.
    /// </summary>
    [Serializable]
    public sealed class ObjectData
    {
        #region Public Fields

        /// <summary>
        /// Default path to show when object is not used.
        /// </summary>
        public const string DEFAULT_PATH = "UNUSED";

        /// <summary>
        /// Database index of the object.
        /// </summary>
        [SerializeField] public int index = -1;

        /// <summary>
        /// Resource path of the object.
        /// </summary>
        [SerializeField] public string path = DEFAULT_PATH;

        /// <summary>
        /// Prefab of the object.
        /// </summary>
        [SerializeField] public GameObject prefab = null;

        /// <summary>
        /// How many of this object can exist in one level.
        /// </summary>
        [SerializeField] public int limit = -1;

        /// <summary>
        /// Category of the object.
        /// </summary>
        [SerializeField] public ObjectDatabase.Category 
            category = ObjectDatabase.Category.None;

        /// <summary>
        /// Editor snapping mode of the object.
        /// </summary>
        [SerializeField] public ObjectDatabase.SnapMode snapMode = 
            ObjectDatabase.SnapMode.Center;

        #endregion
        #region Public Methods

        /// <summary>
        /// Index constructor.
        /// </summary>
        public ObjectData(int index) 
        {
            this.index = index;
        }

        #endregion
    }
}