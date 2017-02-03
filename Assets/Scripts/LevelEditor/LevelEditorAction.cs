using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public abstract class LevelEditorAction {

	public abstract void Undo();

    public abstract void Redo();
}

[Serializable]
public class CreateObjectAction : LevelEditorAction {

    GameObject _object;

    [SerializeField] byte _createdObjectIndex;

    [SerializeField] Vector3 _position;

    public CreateObjectAction (byte index, Vector3 position, GameObject obj) {
        _createdObjectIndex = index;
        _position = position;
        _object = obj;
    }

    public override void Undo () {
        LevelManager.Instance.DeleteObject (_object, false);
    }

    public override void Redo () {
        LevelManager.Instance.CreateObject (_createdObjectIndex, _position, false);
    }

    public override string ToString() {
        return string.Format("CreateObjectAction({0}, {1})", _createdObjectIndex, _position.ToString());
    }
}

[Serializable]
public class DeleteObjectAction : LevelEditorAction {

    [SerializeField] Vector3 _position;

    [SerializeField] byte _deletedObjectIndex;

    GameObject _object;

    public DeleteObjectAction (GameObject obj, byte index) {
        _object = obj;
        _position = obj.transform.position;
        _deletedObjectIndex = index;
    }

    public override void Undo() {
        LevelManager.Instance.CreateObject (_deletedObjectIndex, _position, false);
    }

    public override void Redo () {
        LevelManager.Instance.DeleteObject (_object, false);
    }

    public override string ToString() {
        return string.Format ("DeleteObjectAction({0})", _position.ToString());
    }
}

//public class MoveObjectAction : LevelEditorAction {



//}
