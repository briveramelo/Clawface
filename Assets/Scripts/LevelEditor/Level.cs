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
        public List<SerializeableKeyValuePair<string, string>> attributes = 
            new List<SerializeableKeyValuePair<string, string>>();

        public ObjectAttributes(byte index, Vector3 position, int yRotation) {
            attributes.Add (new SerializeableKeyValuePair<string, string> ("INDEX", index.ToString()));
            attributes.Add (new SerializeableKeyValuePair<string, string> ("POSX", position.x.ToString()));
            attributes.Add (new SerializeableKeyValuePair<string, string> ("POSY", position.y.ToString()));
            attributes.Add (new SerializeableKeyValuePair<string, string> ("POSZ", position.z.ToString()));
            attributes.Add (new SerializeableKeyValuePair<string, string> ("ROTX", 0.ToString()));
            attributes.Add (new SerializeableKeyValuePair<string, string> ("ROTY", yRotation.ToString()));
            attributes.Add (new SerializeableKeyValuePair<string, string> ("ROTZ", 0.ToString()));
        }

        public Vector3 Position {
            get {
                return new Vector3(
                    GetAttributeAsFloat ("POSX"),
                    GetAttributeAsFloat ("POSY"),
                    GetAttributeAsFloat ("POSZ"));
            }
        }

        public byte GetAttributeAsByte (string attribName) {
            foreach (var attribute in attributes)
                if (attribute.Key == attribName) {
                    byte result;
                    if (byte.TryParse (attribute.Value, out result))
                        return result;
                    else
                        throw new NullReferenceException ("Failed to parse attribute \'" + attribName + "\' as byte!");
                }

            throw new NullReferenceException ("Attribute \'" + attribName + "\' not found!");
        }

        public float GetAttributeAsFloat (string attribName) {
            foreach (var attribute in attributes)
                if (attribute.Key == attribName) {
                    float result;
                    if (float.TryParse (attribute.Value, out result))
                        return result;
                    else throw new NullReferenceException ("Failed to parse attribute \'" + attribName + "\' as float!");
                }

            throw new NullReferenceException ("Attribute \'" + attribName + "\' not found!");
        }

        public string GetAttributeAsString (string attribName) {
            foreach (var attribute in attributes)
                if (attribute.Key == attribName)
                    return attribute.Value;

            throw new NullReferenceException ("Attribute \'" + attribName + "\' not found!");
        }

        public void SetAttribute (string attribName, string newValue) {

            foreach (var attribute in attributes) {
                if (attribute.Key == attribName) {
                    attribute.SetValue(newValue);
                    return;
                }
            }

            throw new NullReferenceException ("Attribute \'" + attribName + "\' not found!");
        }

        public void SetPosition (Vector3 pos) {
            SetAttribute ("POSX", pos.x.ToString());
            SetAttribute ("POSY", pos.y.ToString());
            SetAttribute ("POSZ", pos.z.ToString());
        }
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

    [NonSerialized]
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
