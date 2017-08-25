// Level.cs

using System;

using UnityEngine;

namespace Turing.LevelEditor
{

    /// <summary>
    /// Serializable level class for easy network transfer.
    /// </summary>
    [Serializable]
    public class Level
    {
        #region Constants

        /// <summary>
        /// Default level name.
        /// </summary>
        const string _DEFAULT_NAME = "New Level";


        public const int MAX_FLOORS = 5;


        public const int FLOOR_WIDTH = 64;
        public const int FLOOR_DEPTH = 64;
        public const int FLOOR_HEIGHT = 10;

        #endregion
        #region Vars

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
        /// Direct accessor (read-only).
        /// </summary>
        public Floor this[int index] {
            get { return _floors[index]; }
        }

        /// <summary>
        /// Places an object in the level.
        /// </summary>
        /// <param name="index">Index of object.</param>
        public void AddObject(int index, int floor, Vector3 position,
            float yRotation) {
            _floors[floor].AddObject(index, position, yRotation);
        }

        /// <summary>
        /// Removes an object from the level.
        /// </summary>
        public void DeleteObject(int floor, int index) {
            _floors[floor].DeleteObject(index);
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
        public class Floor
        {
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

            /// <summary>
            /// Adds an object to this floor.
            /// </summary>
            public void AddObject(int index, Vector3 position,
                float yRotation) {
                _objects.Add(new ObjectAttributes((byte)index, position,
                    yRotation));
            }

            /// <summary>
            /// Deletes an object from this floor.
            /// </summary>
            public void DeleteObject(int index) {
                _objects.RemoveAt(index);
            }

            /// <summary>
            /// Returns the array of objects on this floor.
            /// </summary>
            public ObjectArray Objects { get { return _objects; } }
        }

        /// <summary>
        /// Serializable class wrapper for an array of floors.
        /// </summary>
        [Serializable]
        public class FloorArray
        {
            /// <summary>
            /// All floors in this array.
            /// </summary>
            [SerializeField]
            Floor[] _floors = new Floor[MAX_FLOORS];

            /// <summary>
            /// Default constructor.
            /// </summary>
            public FloorArray() {
                for (int i = 0; i < MAX_FLOORS; i++)
                    _floors[i] = new Floor();
            }

            /// <summary>
            /// Direct accessor.
            /// </summary>
            public Floor this[int i] {
                get { return _floors[i]; }
                set { _floors[i] = value; }
            }
        }

        /// <summary>
        /// Serializable class wrapper for an array of objects.
        /// </summary>
        [Serializable]
        public class ObjectArray
        {
            /// <summary>
            /// Max number of objects on each floor.
            /// </summary>
            const int _MAX_OBJECTS_PER_FLOOR = 1024;

            /// <summary>
            /// Array of objects.
            /// </summary>
            [SerializeField]
            ObjectAttributes[] _objects =
                new ObjectAttributes[_MAX_OBJECTS_PER_FLOOR];

            /// <summary>
            /// Direct accessor.
            /// </summary>
            public ObjectAttributes this[int i] {
                get { return _objects[i]; }
                set { _objects[i] = value; }
            }

            /// <summary>
            /// Returns the maximum number of objects on this floor.
            /// </summary>
            public int Length { get { return _MAX_OBJECTS_PER_FLOOR; } }

            /// <summary>
            /// Adds an object to this floor.
            /// This is O(n), so don't call this too frequently.
            /// </summary>
            public void Add(ObjectAttributes obj) {
                if (obj.Index == byte.MaxValue)
                    throw new IndexOutOfRangeException(
                        "Invalid index!" + obj.Index);

                // Look for an empty slot
                for (int i = 0; i < _MAX_OBJECTS_PER_FLOOR; i++)
                    if (_objects[i] == null ||
                        _objects[i].Index == byte.MaxValue) {
                        _objects[i] = obj;
                        return;
                    }

                throw new IndexOutOfRangeException("Too many objects!");
            }

            /// <summary>
            /// Removes the object at the given index.
            /// </summary>
            public void RemoveAt(int i) {
                _objects[i] = null;
            }
        }

        /// <summary>
        /// Serializable class wrapper for an array of attributes.
        /// </summary>
        [Serializable]
        public class AttributeArray
        {
            /// <summary>
            /// Maximum number of "special" attributes an object can have.
            /// </summary>
            public const int MAX_OBJECT_ATTRIBUTES = 8;

            /// <summary>
            /// Array of special attributes.
            /// </summary>
            [SerializeField]
            SerializeableStringPair[] _attributes =
                new SerializeableStringPair[MAX_OBJECT_ATTRIBUTES];

            /// <summary>
            /// Direct accessor.
            /// </summary>
            public SerializeableStringPair this[int i] {
                get { return _attributes[i]; }
                set { _attributes[i] = value; }
            }

            /// <summary>
            /// Adds a special property to this object.
            /// </summary>
            public void Add(SerializeableStringPair pair) {
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

        /// <summary>
        /// Serializable class to represent object attributes.
        /// </summary>
        [Serializable]
        public class ObjectAttributes
        {
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

            /// <summary>
            /// Special attributes.
            /// </summary>
            [SerializeField]
            AttributeArray _attributes = new AttributeArray();

            /// <summary>
            /// Default constructor.
            /// !! Needed for proper serialization !!
            /// </summary>
            public ObjectAttributes() {
                _index = byte.MaxValue;
            }

            public ObjectAttributes(byte index, Vector3 position, float yRotation) {
                _index = index;
                _posX = position.x;
                _posY = position.y;
                _posZ = position.z;
                _rotY = yRotation;
            }

            /// <summary>
            /// Returns the index of this object (read-only).
            /// </summary>
            public byte Index { get { return _index; } }

            /// <summary>
            /// Returns the 3D position of this object (read-only).
            /// </summary>
            public Vector3 Position {
                get { return new Vector3(_posX, _posY, _posZ); }
            }

            /// <summary>
            /// Returns the euler rotation of this object (read-only).
            /// </summary>
            public Vector3 EulerRotation {
                get { return new Vector3(_rotX, _rotY, _rotZ); }
            }

            /// <summary>
            /// Gets/sets the y-rotation of this object.
            /// </summary>
            public float RotationY {
                get { return _rotY; }
                set { _rotY = value; }
            }

            /// <summary>
            /// Returns the 3D scale of this object (read-only).
            /// </summary>
            public Vector3 Scale {
                get {
                    return new Vector3(_scaleX, _scaleY, _scaleZ);
                }
            }

            /// <summary>
            /// Gets/sets the x-scale of this object.
            /// </summary>
            public float ScaleX {
                get { return _scaleX; }
                set { _scaleX = value; }
            }

            /// <summary>
            /// Gets/sets the y-scale of this object.
            /// </summary>
            public float ScaleY {
                get { return _scaleY; }
                set { _scaleY = value; }
            }

            /// <summary>
            /// Gets/sets the z-scale of this object.
            /// </summary>
            public float ScaleZ {
                get { return _scaleZ; }
                set { _scaleZ = value; }
            }

            /// <summary>
            /// Returns the value of the attribute with the given name as a byte.
            /// </summary>
            public byte GetAttributeAsByte(string attribName) {
                for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++) {
                    var attribute = _attributes[i];
                    if (attribute.Key == attribName) {
                        byte result;
                        if (byte.TryParse(attribute.Value, out result))
                            return result;
                        else
                            throw new NullReferenceException(
                                "Failed to parse attribute \'" + attribName +
                                "\' as byte!");
                    }
                }

                throw new NullReferenceException("Attribute \'" + attribName +
                    "\' not found!");
            }

            /// <summary>
            /// Returns the value of the attribute with the given name as a float.
            /// </summary>
            public float GetAttributeAsFloat(string attribName) {
                for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++) {
                    var attribute = _attributes[i];
                    if (attribute.Key == attribName) {
                        float result;
                        if (float.TryParse(attribute.Value, out result))
                            return result;
                        else throw new NullReferenceException(
                            "Failed to parse attribute \'" + attribName +
                            "\' as float!");
                    }
                }

                throw new NullReferenceException("Attribute \'" + attribName +
                    "\' not found!");
            }

            /// <summary>
            /// Returns the value of the attribute with the given name as a string.
            /// </summary>
            public string GetAttributeAsString(string attribName) {
                for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++) {
                    var attribute = _attributes[i];
                    if (attribute.Key == attribName)
                        return attribute.Value;
                }

                throw new NullReferenceException("Attribute \'" + attribName +
                    "\' not found!");
            }

            /// <summary>
            /// Sets the value of the attribute with the given name.
            /// </summary>
            public void SetAttribute(string attribName, string newValue) {
                for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++) {
                    var attribute = _attributes[i];
                    if (attribute.Key == attribName) {
                        attribute.SetValue(newValue);
                        return;
                    }
                }

                throw new NullReferenceException("Attribute \'" + attribName +
                    "\' not found!");
            }

            /// <summary>
            /// Sets the 3D position of this object.
            /// </summary>
            public void SetPosition(Vector3 pos) {
                _posX = pos.x;
                _posY = pos.y;
                _posZ = pos.z;
            }

            /// <summary>
            /// Sets the rotation of an object with euler angles.
            /// </summary>
            public void SetEulerRotation(Vector3 euler) {
                _rotX = euler.x;
                _rotY = euler.y;
                _rotZ = euler.z;
            }

            /// <summary>
            /// Sets the 3D local scale of an object.
            /// </summary>
            public void Set3DScale(Vector3 scale) {
                _scaleX = scale.x;
                _scaleY = scale.y;
                _scaleZ = scale.z;
            }

            public override string ToString() {
                return _attributes.ToString();
            }
        }

        #endregion
    }
}