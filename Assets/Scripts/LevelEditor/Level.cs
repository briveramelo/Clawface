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
    FloorArray _floors = new FloorArray();

    [NonSerialized]
    public LevelEvent onLevelComplete;

    #endregion
    #region Constructors

    /// <summary>
    /// Default constructor -- inits floor structures.
    /// </summary>
    public Level() {
        _name = _DEFAULT_NAME;
        _floors = new FloorArray();
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
    #region Nested Classes

    /// <summary>
    /// Floor class holds 3D object data.
    /// </summary>
    [Serializable]
    public class Floor {

        /// <summary>
        /// Byte values at each tile.
        /// </summary>
        [SerializeField]
        ObjectArray _objects = new ObjectArray();

        /// <summary>
        /// Direct accessor.
        /// </summary>
        public ObjectAttributes this[int i] {
            get { return _objects[i]; }
            set { _objects[i] = value; }
        }

        public void AddObject(int index, Vector3 position, int yRotation) {
            _objects.Add(new ObjectAttributes((byte)index, position, yRotation));
        }

        public void DeleteObject (int index) {
            _objects.RemoveAt (index);
        }

        public ObjectArray Objects { get { return _objects; } }
    }

    [Serializable]
    public class FloorArray {

        [SerializeField]
        Floor[] _floors = new Floor[MAX_FLOORS];
  
        public FloorArray () {
            for (int i = 0; i < MAX_FLOORS; i++)
                _floors[i] = new Floor();
        }

        public Floor this[int i] {
            get { return _floors[i]; }
            set { _floors[i] = value; }
        }
    }

    [Serializable]
    public class ObjectArray {
        const int _MAX_OBJECTS_PER_FLOOR = 1024;

        [SerializeField]
        ObjectAttributes[] _objects = new ObjectAttributes[_MAX_OBJECTS_PER_FLOOR];

        public ObjectAttributes this[int i] {
            get { return _objects[i]; }
            set { _objects[i] = value; }
        }

        public int Length { get { return _MAX_OBJECTS_PER_FLOOR; } }

        public void Add (ObjectAttributes obj) {
            if (obj.Index == byte.MaxValue) throw new IndexOutOfRangeException ("Invalid index!");

            for (int i = 0; i < _MAX_OBJECTS_PER_FLOOR; i++)
                if (_objects[i] == null || _objects[i].Index == byte.MaxValue) {
                    _objects[i] = obj;
                    return;
                }

            throw new IndexOutOfRangeException ("Too many objects!");
        }

        public void RemoveAt (int i) {
            _objects[i] = null;
        }
    }

    [Serializable]
    public class AttributeArray {
        public const int MAX_OBJECT_ATTRIBUTES = 8;

        [SerializeField]
        SerializeableStringPair[] _attributes = 
            new SerializeableStringPair[MAX_OBJECT_ATTRIBUTES];

        public SerializeableStringPair this[int i] {
            get { return _attributes[i]; }
            set { _attributes[i] = value; }
        }

        public void Add (SerializeableStringPair pair) {
            for (int i = 0; i < MAX_OBJECT_ATTRIBUTES; i++)
                if (_attributes[i] == null) {
                    _attributes[i] = pair;
                    return;
                }
        }

        public override string ToString() {
            string result = "";
            for (int i = 0; i < MAX_OBJECT_ATTRIBUTES; i++) {
                var attribute = _attributes[i];
                if (attribute == null) continue;

                if (i > 0) result += "\n";
                result += attribute.ToString();
            }
            return result;
        }
    }

    [Serializable]
    public class ObjectAttributes {

        [SerializeField] byte _index = byte.MaxValue;
        [SerializeField] float _posX;
        [SerializeField] float _posY;
        [SerializeField] float _posZ;
        [SerializeField] float _rotX = 0f;
        [SerializeField] float _rotY;
        [SerializeField] float _rotZ = 0f;
        [SerializeField] float _scaleX = 1f;
        [SerializeField] float _scaleY = 1f;
        [SerializeField] float _scaleZ = 1f;

        [SerializeField]
        AttributeArray _attributes = new AttributeArray();

        public ObjectAttributes () {
            _index = byte.MaxValue;
        }

        public ObjectAttributes(byte index, Vector3 position, int yRotation) {
            _index = index;
            _posX = position.x;
            _posY = position.y;
            _posZ = position.z;
            _rotY = yRotation;
            //Debug.Log (this.ToString());
        }

        public byte Index {
            get { return _index; }
            set { _index = value; }
        }

        public Vector3 Position {
            get { return new Vector3(_posX, _posY, _posZ); }
        }

        public Vector3 EulerRotation {
            get { return new Vector3 (
                _rotX, _rotY, _rotZ); }
        }

        public float YRotation {
            get { return _rotY; }
            set { _rotY = value; }
        }

        public byte GetAttributeAsByte (string attribName) {
            for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++) {
                var attribute = _attributes[i];
                if (attribute.Key == attribName) {
                    byte result;
                    if (byte.TryParse (attribute.Value, out result))
                        return result;
                    else
                        throw new NullReferenceException ("Failed to parse attribute \'" + attribName + "\' as byte!");
                }
            }

            throw new NullReferenceException ("Attribute \'" + attribName + "\' not found!");
        }

        public float GetAttributeAsFloat (string attribName) {
            for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++) {
                var attribute = _attributes[i];
                if (attribute.Key == attribName) {
                    float result;
                    if (float.TryParse (attribute.Value, out result))
                        return result;
                    else throw new NullReferenceException ("Failed to parse attribute \'" + attribName + "\' as float!");
                }
            }

            throw new NullReferenceException ("Attribute \'" + attribName + "\' not found!");
        }

        public string GetAttributeAsString (string attribName) {
            for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++) {
                var attribute = _attributes[i];
                if (attribute.Key == attribName)
                    return attribute.Value;
            }

            throw new NullReferenceException ("Attribute \'" + attribName + "\' not found!");
        }

        public void SetAttribute (string attribName, string newValue) {
            for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++) {
                var attribute = _attributes[i];
                if (attribute.Key == attribName) {
                    attribute.SetValue(newValue);
                    return;
                }
            }

            throw new NullReferenceException ("Attribute \'" + attribName + "\' not found!");
        }

        public void SetPosition (Vector3 pos) {
            _posX = pos.x;
            _posY = pos.y;
            _posZ = pos.z;
        }

        public override string ToString() {
            return _attributes.ToString();
        }
    }

    [Serializable]
    public class LevelEvent : UnityEvent { }

    #endregion
}
