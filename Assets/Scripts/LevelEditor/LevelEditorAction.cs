//// LevelEditorAction.cs

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public enum ActionType {
//    None,
//    Normal,
//    Undo,
//    Redo
//}

///// <summary>
///// Base class for all actions performed in the editor so that they can be
///// undone and redone.
///// </summary>
//[Serializable]
//public abstract class LevelEditorAction {

//    /// <summary>
//    /// Undo the action performed.
//    /// </summary>
//	public abstract void Undo();

//    /// <summary>
//    /// Redo the action performed.
//    /// </summary>
//    public abstract void Redo();

//    public abstract string ToShortString();
//}

///// <summary>
///// Object placement action.
///// </summary>
//[Serializable]
//public class CreateObjectAction : LevelEditorAction {

//    GameObject _object;

//    [SerializeField] byte _createdObjectIndex;

//    [SerializeField] Vector3 _position;

//    public CreateObjectAction (byte index, GameObject obj) {
//        _createdObjectIndex = index;
//        _position = obj.transform.position;
//        _object = obj;
//    }

//    public override void Undo () {
//        LevelManager.Instance.DeleteObject (_object, ActionType.Undo);
//    }

//    public override void Redo () {
//        LevelManager.Instance.CreateObject (_createdObjectIndex, _position, ActionType.Redo);
//    }

//    public override string ToString() {
//        return string.Format("CreateObjectAction({0}, {1})", _createdObjectIndex, _position.ToString());
//    }

//    public override string ToShortString() {
//        return string.Format("NEW | I: {0} | POS: {1}", _createdObjectIndex, _position.ToString());
//    }
//}

///// <summary>
///// Erase object action.
///// </summary>
//[Serializable]
//public class DeleteObjectAction : LevelEditorAction {

//    [SerializeField] Vector3 _position;

//    [SerializeField] byte _deletedObjectIndex;

//    GameObject _object;

//    public DeleteObjectAction (GameObject obj, byte index) {
//        _object = obj;
//        _position = obj.transform.position;
//        _deletedObjectIndex = index;
//    }

//    public override void Undo() {
//        LevelManager.Instance.CreateObject (_deletedObjectIndex, _position, ActionType.Undo);
//    }

//    public override void Redo () {
//        LevelManager.Instance.DeleteObject (_object, ActionType.Redo);
//    }

//    public override string ToString() {
//        return string.Format ("DeleteObjectAction({0})", _position.ToString());
//    }

//    public override string ToShortString() {
//        return string.Format ("DEL | I: {0} | POS: {1}", _deletedObjectIndex, _position.ToString());
//    }
//}

////public class MoveObjectAction : LevelEditorAction {



////}
