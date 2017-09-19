// CreateObjectAction.cs
// Author: Aaron

using System;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Object placement action.
    /// </summary>
    [Serializable]
    public sealed class CreateObjectAction : LevelEditorAction
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Index of the created object.
        /// </summary>
        [SerializeField] byte createdObjectIndex;

        /// <summary>
        /// Position of the created object.
        /// </summary>
        [SerializeField] Vector3 position;

        #endregion
        #region Private Fields

        /// <summary>
        /// The created object.
        /// </summary>
        GameObject obj;

        #endregion
        #region Public Methods

        public CreateObjectAction(byte index, GameObject obj)
        {
            createdObjectIndex = index;
            position = obj.transform.position;
            this.obj = obj;
        }

        public override void Undo()
        {
            LevelManager.Instance.DeleteObject(obj, ActionType.Undo);
        }

        public override void Redo()
        {
            LevelManager.Instance.CreateObject(createdObjectIndex,
                position, ActionType.Redo);
        }

        public override string ToString()
        {
            return string.Format("CreateObjectAction({0}, {1})",
                createdObjectIndex, position.ToString());
        }

        public override string ToShortString()
        {
            return string.Format("NEW | I: {0} | POS: {1}",
                createdObjectIndex, position.ToString());
        }

        #endregion
    }
}