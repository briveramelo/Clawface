// DeleteObjectAction.cs
// Author: Aaron

using System;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Erase object action.
    /// </summary>
    [Serializable]
    public sealed class DeleteObjectAction : LevelEditorAction
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Position of the deleted object.
        /// </summary>
        [SerializeField] Vector3 position;

        /// <summary>
        /// Index of the deleted object.
        /// </summary>
        [SerializeField] byte deletedObjectIndex;

        #endregion
        #region Private Fields

        /// <summary>
        /// Deleted object.
        /// </summary>
        GameObject obj;

        #endregion
        #region Public Methods

        public DeleteObjectAction(GameObject obj, byte index)
        {
            this.obj = obj;
            position = obj.transform.position;
            deletedObjectIndex = index;
        }

        public override void Undo()
        {
            LevelManager.Instance.CreateObject(deletedObjectIndex,
                position, ActionType.Undo);
        }

        public override void Redo()
        {
            LevelManager.Instance.DeleteObject(obj, ActionType.Redo);
        }

        public override string ToString()
        {
            return string.Format("DeleteObjectAction({0})",
                position.ToString());
        }

        public override string ToShortString()
        {
            return string.Format("DEL | I: {0} | POS: {1}",
                deletedObjectIndex, position.ToString());
        }

        #endregion
    }
}