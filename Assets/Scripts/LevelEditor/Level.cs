// Level.cs

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

/// <summary>
/// Serializable level struct for easy network transfer.
/// </summary>
[Serializable]
public class Level {

    #region Nested Classes

    /// <summary>
    /// Floor class holds 3D object data.
    /// </summary>
    [Serializable]
    public class Floor {

        const int _MAX_OBJECTS = 1024;

        /// <summary>
        /// Byte values at each tile.
        /// </summary>
        //public ObjectAttributes[,,] values;
        [SerializeField]
        List<ObjectAttributes> _values;

        /// <summary>
        /// Default constructor -- initializes all byte values to byte.MaxValue
        /// which represents empty space.
        /// </summary>
        public Floor() {
            _values = new List<ObjectAttributes>();
        }

        /// <summary>
        /// Direct accessor.
        /// </summary>
        public ObjectAttributes this[int i] {
            get {
                if (i < 0 || i >= _values.Count) {
                    Debug.LogError("Invalid index: " + i);
                    return default(ObjectAttributes);
                }
                return _values[i];
            }
            set { _values[i] = value; }
        }

        public void AddObject(int index, Vector3 position, int yRotation) {
            _values.Add(new ObjectAttributes((byte)index, position, yRotation));
        }

        public void DeleteObject (int index) {
            _values.RemoveAt (index);
        }

        public List<ObjectAttributes> Objects { get { return _values; } }
    }

    [Serializable]
    public class ObjectAttributes {

        [SerializeField]
        public byte index;

        [SerializeField]
        public float x;

        [SerializeField]
        public float y;

        [SerializeField]
        public float z;

        [SerializeField]
        public int yRotation;

        public ObjectAttributes(byte index, Vector3 position, int yRotation) {
            this.index = index;
            this.yRotation = yRotation;
            this.x = position.x;
            this.y = position.y;
            this.z = position.z;
        }

        public Vector3 Position { get { return new Vector3(x, y, z); } }
    }

    [Serializable]
    public class LevelEvent : UnityEvent { }

    #endregion
    #region Vars

    const string _DEFAULT_NAME = "New Level";
    public const int MAX_FLOORS = 5;
    public const int FLOOR_WIDTH = 64;
    public const int FLOOR_DEPTH = 64;
    public const int FLOOR_HEIGHT = 10;

    /// <summary>
    /// Name of the level.
    /// </summary>
    [SerializeField]
    string _name;

    /// <summary>
    /// All floor structures in the level.
    /// </summary>
    [SerializeField]
    Floor[] _floors;

    [SerializeField]
    float _playerSpawnX = 0f;

    [SerializeField]
    float _playerSpawnY = 0f;

    [SerializeField]
    float _playerSpawnZ = 0f;

    public LevelEvent onLevelComplete;

    #endregion
    #region Constructors

    /// <summary>
    /// Default constructor -- inits floor structures.
    /// </summary>
    public Level() {
        _name = _DEFAULT_NAME;

        _floors = new Floor[MAX_FLOORS];
        for (int i = 0; i < MAX_FLOORS; i++) {
            _floors[i] = new Floor();
        }
    }

    #endregion
    #region Properties

    /// <summary>
    /// Gets/sets the name of this level.
    /// </summary>
    public string Name {
        get { return _name; }
        set { _name = value; }
    }

    #endregion
    #region Methods

    /// <summary>
    /// Direct accessor.
    /// </summary>
    public Floor this[int index] {
        get { return _floors[index]; }
    }

    public Vector3 PlayerSpawnPosition {
        get {
            return new Vector3(
                _playerSpawnX,
                _playerSpawnY,
                _playerSpawnZ);
        }
    }

    public void SetPlayerSpawnPosition(Vector3 position) {
        _playerSpawnX = position.x;
        _playerSpawnY = position.y;
        _playerSpawnZ = position.z;
    }

    /// <summary>
    /// Places an object in the level.
    /// </summary>
    /// <param name="index">Index of object.</param>
    public void AddObject(int index, int floor, Vector3 position, int yRotation) {
        _floors[floor].AddObject(index, position, yRotation);
    }

    /// <summary>
    /// Removes an object from the level.
    /// </summary>
    public void DeleteObject(int floor, int index) {
        _floors[floor].DeleteObject (index);
    }

    /// <summary>
    /// Creates a LevelAsset wrapper for this level.
    /// </summary>
    public LevelAsset ToLevelAsset() {
        var asset = ScriptableObject.CreateInstance<LevelAsset>();
        asset.Pack(this);
        return asset;
    }

    #endregion
    #region Overrides

    public override string ToString() {
        return _name;
    }

    #endregion
}
