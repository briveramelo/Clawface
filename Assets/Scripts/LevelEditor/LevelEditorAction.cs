using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public abstract class LevelEditorAction {

	public abstract void Undo();

    public abstract void Redo();

    public abstract LevelEditorAction ToUndo ();

    public abstract LevelEditorAction ToRedo ();
}

[Serializable]
public class CreateObjectAction : LevelEditorAction {

    [SerializeField]
    byte _createdObjectIndex;

    [SerializeField]
    Level.CoordinateSet _coords;

    public CreateObjectAction (byte index, Level.CoordinateSet coords) {
        _createdObjectIndex = index;
        _coords = coords;
    }

    public override void Undo () {
        LevelManager.Instance.DeleteObject (_coords, false);
    }

    public override void Redo () {
        LevelManager.Instance.CreateObject (_createdObjectIndex, _coords, false);
    }

    public override LevelEditorAction ToUndo() {
        return new DeleteObjectAction (_createdObjectIndex, _coords);
    }

    public override LevelEditorAction ToRedo() {
        return new CreateObjectAction (_createdObjectIndex, _coords);
    }

    public override string ToString() {
        return string.Format("CreateObjectAction({0}, {1})", _createdObjectIndex, _coords.ToString());
    }
}

[Serializable]
public class DeleteObjectAction : LevelEditorAction {

    [SerializeField]
    Level.CoordinateSet _coords;

    [SerializeField]
    byte _deletedObjectIndex;

    public DeleteObjectAction (byte index, Level.CoordinateSet coords) {
        _coords = coords;
        _deletedObjectIndex = index;
    }

    public override void Undo() {
        LevelManager.Instance.CreateObject (_deletedObjectIndex, _coords, false);
    }

    public override void Redo () {
        LevelManager.Instance.DeleteObject (_coords, false);
    }

    public override LevelEditorAction ToUndo() {
        return new CreateObjectAction (_deletedObjectIndex, _coords);
    }

    public override LevelEditorAction ToRedo() {
        return new DeleteObjectAction (_deletedObjectIndex, _coords);
    }

    public override string ToString() {
        return string.Format ("DeleteObjectAction({0})", _coords.ToString());
    }
}

//public class MoveObjectAction : LevelEditorAction {



//}
