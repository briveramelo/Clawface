// LevelEditorAction.cs
// Author: Aaron

using System;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Base class for all actions performed in the editor so that they can be
    /// undone and redone.
    /// </summary>
    [Serializable]
    public abstract class LevelEditorAction
    {
        #region Public Methods

        /// <summary>
        /// Undo the action performed.
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// Redo the action performed.
        /// </summary>
        public abstract void Redo();

        /// <summary>
        /// Returns a short string representing this action.
        /// </summary>
        public abstract string ToShortString();

        #endregion
        #region Public Structures

        /// <summary>
        /// Type of level editor action.
        /// </summary>
        public enum ActionType
        {
            None,
            Normal,
            Undo,
            Redo
        }
    }

    #endregion
}