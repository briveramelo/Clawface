// LevelObject.cs

using UnityEngine;

[ExecuteInEditMode]
public class LevelObject : SingletonMonoBehaviour<LevelObject> {

    [SerializeField] Level _level;

    public Level Level {
        get { return _level; }
        set { _level = value; }
    }

	new void Awake() {
        if (Instance != null)
            LevelManager.Instance.DestroyLoadedObject (Instance.gameObject);

        base.Awake();
    } 
}
