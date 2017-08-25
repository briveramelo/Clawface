// LevelEditorAction.cs

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
        GameObject _object;

        /// <summary>
        /// Index of the created object.
        /// </summary>
        [SerializeField]
        byte _createdObjectIndex;

        /// <summary>
        /// Position of the created object.
        /// </summary>
        [SerializeField]
        Vector3 _position;

        #endregion
        #region Constructors

        public CreateObjectAction(byte index, GameObject obj) {
            _createdObjectIndex = index;
            _position = obj.transform.position;
            _object = obj;
        }

        #endregion
        #region LevelEditorAction Implementation

        public override void Undo() {
            LevelManager.Instance.DeleteObject(_object, ActionType.Undo);
        }

        public override void Redo() {
            LevelManager.Instance.CreateObject(_createdObjectIndex, _position, ActionType.Redo);
        }

        public override string ToString() {
            return string.Format("CreateObjectAction({0}, {1})", _createdObjectIndex, _position.ToString());
        }

        public override string ToShortString() {
            return string.Format("NEW | I: {0} | POS: {1}", _createdObjectIndex, _position.ToString());
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
        GameObject _object;

        /// <summary>
        /// Position of the deleted object.
        /// </summary>
        [SerializeField]
        Vector3 _position;

        /// <summary>
        /// Index of the deleted object.
        /// </summary>
        [SerializeField]
        byte _deletedObjectIndex;

        #endregion
        #region Constructors

        public DeleteObjectAction(GameObject obj, byte index) {
            _object = obj;
            _position = obj.transform.position;
            _deletedObjectIndex = index;
        }

        #endregion
        #region LevelEditorAction Implementation

        public override void Undo() {
            LevelManager.Instance.CreateObject(_deletedObjectIndex, _position, ActionType.Undo);
        }

        public override void Redo() {
            LevelManager.Instance.DeleteObject(_object, ActionType.Redo);
        }

        public override string ToString() {
            return string.Format("DeleteObjectAction({0})", _position.ToString());
        }

        public override string ToShortString() {
            return string.Format("DEL | I: {0} | POS: {1}", _deletedObjectIndex, _position.ToString());
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
        [SerializeField]
        GameObject _obj;

        /// <summary>
        /// Original position of object.
        /// </summary>
        [SerializeField]
        Vector3 _posMovedFrom;

        /// <summary>
        /// New position of object.
        /// </summary>
        [SerializeField]
        Vector3 _posMovedTo;

        #endregion
        #region Constructors

        public MoveObjectAction(GameObject obj, Vector3 oldPos, Vector3 newPos) {
            _obj = obj;
            _posMovedFrom = oldPos;
            _posMovedTo = newPos;
        }

        #endregion
        #region LevelEditorAction Implementation

        public override void Undo() {
            LevelManager.Instance.MoveObject(_obj, _posMovedTo, _posMovedFrom, ActionType.Undo);
        }

        public override void Redo() {
            LevelManager.Instance.MoveObject(_obj, _posMovedFrom, _posMovedTo, ActionType.Redo);
        }

        public override string ToShortString() {
            return string.Format("MOV | OLD: {0} | NEW: {1}", _posMovedFrom, _posMovedTo);
        }
    }

    [Serializable]
    public class ChangeObjectNormalAttributeAction : LevelEditorAction
    {

        [SerializeField]
        GameObject _object;

        [SerializeField]
        AttributeChanged _attrib;

        [SerializeField]
        Vector3 _oldValue;

        [SerializeField]
        Vector3 _newValue;

        public enum AttributeChanged
        {
            Position,
            Rotation,
            Scale
        }

        public ChangeObjectNormalAttributeAction(GameObject obj, AttributeChanged attrib, Vector3 newValue) {
            _object = obj;
            _attrib = attrib;

            switch (attrib) {
                case AttributeChanged.Position:
                    _oldValue = _object.transform.position;
                    break;

                case AttributeChanged.Rotation:
                    _oldValue = _object.transform.rotation.eulerAngles;
                    break;

                case AttributeChanged.Scale:
                    _oldValue = _object.transform.localScale;
                    break;
            }

            _newValue = newValue;
        }

        public override void Undo() {
            switch (_attrib) {
                case AttributeChanged.Position:
                    LevelManager.Instance.SetObjectPosition(_object, _oldValue, LevelEditorAction.ActionType.Undo);
                    break;

                case AttributeChanged.Rotation:
                    LevelManager.Instance.SetObjectEulerRotation(_object, _oldValue, LevelEditorAction.ActionType.Undo);
                    break;

                case AttributeChanged.Scale:
                    LevelManager.Instance.SetObject3DScale(_object, _oldValue, LevelEditorAction.ActionType.Undo);
                    break;
            }

            LevelManager.Instance.RecordAttributeChange(_object, _attrib, _newValue, _oldValue, LevelEditorAction.ActionType.Undo);
        }

        public override void Redo() {
            switch (_attrib) {
                case AttributeChanged.Position:
                    LevelManager.Instance.SetObjectPosition(_object, _newValue, LevelEditorAction.ActionType.Redo);
                    break;

                case AttributeChanged.Rotation:
                    LevelManager.Instance.SetObjectEulerRotation(_object, _newValue, LevelEditorAction.ActionType.Redo);
                    break;

                case AttributeChanged.Scale:
                    LevelManager.Instance.SetObject3DScale(_object, _newValue, ActionType.Redo);
                    break;
            }

            LevelManager.Instance.RecordAttributeChange(_object, _attrib, _oldValue, _newValue, ActionType.Undo);
        }

        public override string ToShortString() {
            return string.Format("CHG {0} | Old: {1} | New: {2}", _attrib.ToString().Substring(0, 1), _oldValue.ToString(), _newValue.ToString());
        }
    }

    [Serializable]
    public class ChangeObjectSpecialAttributeAction : LevelEditorAction
    {

        public override void Undo() {
            throw new NotImplementedException();
        }

        public override void Redo() {
            throw new NotImplementedException();
        }

        public override string ToShortString() {
            throw new NotImplementedException();
        }
    }

    #endregion
}