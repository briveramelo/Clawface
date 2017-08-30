// LevelEditorAction.cs
// Author: Aaron

using System;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Base class for all actions performed in the editor so that they can be
    /// undone and redone.
    /// </summary>
    [Serializable]
    public abstract class LevelEditorAction
    {
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

    /// <summary>
    /// Object placement action.
    /// </summary>
    [Serializable]
    public class CreateObjectAction : LevelEditorAction
    {
        #region Vars

        /// <summary>
        /// The created object.
        /// </summary>
        GameObject obj;

        /// <summary>
        /// Index of the created object.
        /// </summary>
        [SerializeField] byte createdObjectIndex;

        /// <summary>
        /// Position of the created object.
        /// </summary>
        [SerializeField] Vector3 position;

        #endregion
        #region Constructors

        public CreateObjectAction(byte index, GameObject obj)
        {
            createdObjectIndex = index;
            position = obj.transform.position;
            this.obj = obj;
        }

        #endregion
        #region LevelEditorAction Implementation

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

    /// <summary>
    /// Erase object action.
    /// </summary>
    [Serializable]
    public class DeleteObjectAction : LevelEditorAction
    {
        #region Vars

        /// <summary>
        /// Deleted object.
        /// </summary>
        GameObject obj;

        /// <summary>
        /// Position of the deleted object.
        /// </summary>
        [SerializeField] Vector3 position;

        /// <summary>
        /// Index of the deleted object.
        /// </summary>
        [SerializeField] byte deletedObjectIndex;

        #endregion
        #region Constructors

        public DeleteObjectAction(GameObject obj, byte index)
        {
            this.obj = obj;
            position = obj.transform.position;
            deletedObjectIndex = index;
        }

        #endregion
        #region LevelEditorAction Implementation

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

    /// <summary>
    /// Move object action.
    /// </summary>
    [Serializable]
    public class MoveObjectAction : LevelEditorAction
    {
        #region Vars

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
        #region Constructors

        public MoveObjectAction(GameObject obj, Vector3 oldPos, Vector3 newPos)
        {
            this.obj = obj;
            posMovedFrom = oldPos;
            posMovedTo = newPos;
        }

        #endregion
        #region LevelEditorAction Implementation

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
    }

    [Serializable]
    public class ChangeObjectNormalAttributeAction : LevelEditorAction
    {
        [SerializeField] GameObject obj;

        [SerializeField] AttributeChanged attrib;

        [SerializeField] Vector3 oldValue;

        [SerializeField] Vector3 newValue;

        public enum AttributeChanged
        {
            Position,
            Rotation,
            Scale
        }

        public ChangeObjectNormalAttributeAction(GameObject obj, 
            AttributeChanged attrib, Vector3 newValue)
        {
            this.obj = obj;
            this.attrib = attrib;

            switch (attrib)
            {
                case AttributeChanged.Position:
                    oldValue = obj.transform.position;
                    break;

                case AttributeChanged.Rotation:
                    oldValue = obj.transform.rotation.eulerAngles;
                    break;

                case AttributeChanged.Scale:
                    oldValue = obj.transform.localScale;
                    break;
            }

            this.newValue = newValue;
        }

        public override void Undo()
        {
            switch (attrib)
            {
                case AttributeChanged.Position:
                    LevelManager.Instance.SetObjectPosition(obj, 
                        oldValue, ActionType.Undo);
                    break;

                case AttributeChanged.Rotation:
                    LevelManager.Instance.SetObjectEulerRotation(obj, 
                        oldValue, ActionType.Undo);
                    break;

                case AttributeChanged.Scale:
                    LevelManager.Instance.SetObject3DScale(obj, 
                        oldValue, ActionType.Undo);
                    break;
            }

            LevelManager.Instance.RecordAttributeChange(obj, 
                attrib, newValue, oldValue, ActionType.Undo);
        }

        public override void Redo()
        {
            switch (attrib)
            {
                case AttributeChanged.Position:
                    LevelManager.Instance.SetObjectPosition(obj, 
                        newValue, ActionType.Redo);
                    break;

                case AttributeChanged.Rotation:
                    LevelManager.Instance.SetObjectEulerRotation(obj,
                        newValue, ActionType.Redo);
                    break;

                case AttributeChanged.Scale:
                    LevelManager.Instance.SetObject3DScale(obj, 
                        newValue, ActionType.Redo);
                    break;
            }

            LevelManager.Instance.RecordAttributeChange(obj, 
                attrib, oldValue, newValue, ActionType.Undo);
        }

        public override string ToShortString()
        {
            return string.Format("CHG {0} | Old: {1} | New: {2}", 
                attrib.ToString().Substring(0, 1), oldValue.ToString(), 
                newValue.ToString());
        }
    }

    [Serializable]
    public class ChangeObjectSpecialAttributeAction : LevelEditorAction
    {
        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override void Redo()
        {
            throw new NotImplementedException();
        }

        public override string ToShortString()
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}