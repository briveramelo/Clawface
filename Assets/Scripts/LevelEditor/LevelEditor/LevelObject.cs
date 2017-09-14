// LevelObject.cs
// Author: Aaron

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Class for a level GameObject.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class LevelObject : EditorSingleton<LevelObject>
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// The level data this LevelObject wraps.
        /// </summary>
        [SerializeField] Level level;

        #endregion
        #region Unity Lifecycle

        new void Awake()
        {
            // Destroy the old instance
            if (Instance != null)
                LevelManager.Instance.DestroyLoadedObject(Instance.gameObject);

            base.Awake();
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Gets/sets the encapsulated level.
        /// </summary>
        public Level Level
        {
            get { return level; }
            set { level = value; }
        }

        #endregion
    }
}