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

    /// <summary>
    /// Finds the given component type in ancestors (parents, grandparents, etc.)
    /// </summary>
    public static T GetComponentInAncestors<T> (this GameObject obj) {
        var parent = obj.transform.parent;
        while (parent != null) {
            var component = parent.GetComponent<T>();
            if (component != null) return component;
            parent = parent.transform.parent;
        }

        return default(T);
    }

    /// <summary>
    /// Copies the settings of a component from one GameObject to another.
    /// </summary>
    public static bool CopyComponentFromGameObject<T> (this GameObject obj, GameObject other) where T : Component {
        T otherComponent = other.GetComponent<T>();
        if (otherComponent == null) return false;

        T thisComponent = obj.GetComponent<T>();
        if (thisComponent == null) thisComponent = obj.AddComponent<T>();

        System.Reflection.FieldInfo[] fields = otherComponent.GetType().GetFields();
        foreach (var field in fields) {
            field.SetValue (thisComponent, field.GetValue (otherComponent));
        }

        return true;
    }

    /// <summary>
    /// Copies collider information from one object to another.
    /// </summary>
    public static bool CopyColliderFromGameObject (this GameObject obj, GameObject other) {
        if (CopyComponentFromGameObject<BoxCollider>(obj, other)) return true;
        if (CopyComponentFromGameObject<SphereCollider>(obj, other)) return true;
        if (CopyComponentFromGameObject<CapsuleCollider>(obj, other)) return true;
        if (CopyComponentFromGameObject<MeshCollider>(obj, other)) return true;
        return false;
    }
}
