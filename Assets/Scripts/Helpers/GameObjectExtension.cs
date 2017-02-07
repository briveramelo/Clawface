// GameObjectExtension.cs

using UnityEngine;

/// <summary>
/// Extension functions for GameObjects.
/// </summary>
public static class GameObjectExtension {

    /// <summary>
    /// Finds a GameObject with a certain name in the children of the given
    /// GameObject.
    /// </summary>
	public static GameObject FindInChildren (this GameObject obj, string name) {
        var childTransforms = obj.GetComponentsInChildren<Transform>();
        foreach (var childTr in childTransforms) {
            var childObj = childTr.gameObject;
            if (childObj.name == name) return childObj;
        }

        return null;
    }
}
