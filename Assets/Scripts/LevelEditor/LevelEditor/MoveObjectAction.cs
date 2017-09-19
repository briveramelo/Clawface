// MoveObjectAction.cs
// Author: Aaron

using System;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Move object action.
    /// </summary>
    [Serializable]
    public sealed class MoveObjectAction : LevelEditorAction
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Object that was moved.
        /// </summary>
        [SerializeField] GameObject obj;

        /// <summary>
        /// Original position of object.
        /// </summary>
        [SerializeField] Vector3 posMovedFrom;

        /// <summary>
        /// New position of object.
        /// </summary>
        [SerializeField] Vector3 posMovedTo;

        #endregion
        #region Public methods

        public MoveObjectAction(GameObject obj, Vector3 oldPos, Vector3 newPos)
        {
            this.obj = obj;
            posMovedFrom = oldPos;
            posMovedTo = newPos;
        }

        public override void Undo()
        {
            LevelManager.Instance.MoveObject(obj, posMovedTo,
                posMovedFrom, ActionType.Undo);
        }

        public override void Redo()
        {
            LevelManager.Instance.MoveObject(obj,
                posMovedFrom, posMovedTo, ActionType.Redo);
        }

        public override string ToShortString()
        {
            return string.Format("MOV | OLD: {0} | NEW: {1}",
                posMovedFrom, posMovedTo);
        }

        #endregion
    }
}