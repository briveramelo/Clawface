﻿// LevelManagerEditor.cs

using UnityEditor;

namespace Turing.LevelEditor
{

    /// <summary>
    /// Custom editor for LevelManager.
    /// </summary>
    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerEditor : Editor
    {
        #region Vars

        /// <summary>
        /// Target LevelManager.
        /// </summary>
        LevelManager _target;

        #endregion
        #region Overrides

        public override void OnInspectorGUI() 
        {
            _target = target as LevelManager;

            EditorGUILayout.LabelField("Level loaded: " + 
                _target.LevelLoaded.ToString());

            base.OnInspectorGUI();
        }

        #endregion
    }
}