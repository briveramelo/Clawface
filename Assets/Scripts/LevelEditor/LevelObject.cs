// LevelObject.cs

using UnityEngine;

namespace Turing.LevelEditor
{

    /// <summary>
    /// Class for a level GameObject.
    /// </summary>
    [ExecuteInEditMode]
    public class LevelObject : SingletonMonoBehaviour<LevelObject>
    {

        #region Vars

        /// <summary>
        /// The level data this LevelObject wraps.
        /// </summary>
        [SerializeField] Level _level;

        #endregion
        #region Unity Callbacks

        new void Awake() {

            // Destroy the old instance
            if (Instance != null)
                LevelManager.Instance.DestroyLoadedObject(Instance.gameObject);

            base.Awake();
        }

        #endregion
        #region Properties

        /// <summary>
        /// Gets/sets the encapsulated level.
        /// </summary>
        public Level Level {
            get { return _level; }
            set { _level = value; }
        }

        #endregion
    }
}