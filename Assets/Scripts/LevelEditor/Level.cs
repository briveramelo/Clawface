// Level.cs
// Author: Aaron

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
        const string DEFAULT_NAME = "New Level";

        public const int LEVEL_WIDTH = 64;
        public const int LEVEL_DEPTH = 64;
        public const int LEVEL_HEIGHT = 10;

        #endregion
        #region Vars

        /// <summary>
        /// Name of the level.
        /// </summary>
        [SerializeField] string name;

        /// <summary>
        /// All objects in the level.
        /// </summary>
        [SerializeField]
        ObjectArray objects = new ObjectArray();

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor -- inits object list.
        /// </summary>
        public Level()
        {
            name = DEFAULT_NAME;
            objects = new ObjectArray();
        }

        #endregion
        #region Properties

        /// <summary>
        /// Gets/sets the name of this level.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion
        #region Methods

        /// <summary>
        /// Direct accessor (read-only).
        /// </summary>
        public ObjectArray Objects { get { return objects; } }

        /// <summary>
        /// Places an object in the level.
        /// </summary>
        /// <param name="index">Index of object.</param>
        public void AddObject(int index, Vector3 position,
            float yRotation) 
        {
            var attribs = new ObjectAttributes ((byte)index,
                position, yRotation);
            objects.Add(attribs);
        }

        /// <summary>
        /// Removes an object from the level.
        /// </summary>
        public void DeleteObject(int index) 
        {
            objects.RemoveAt(index);
        }

        #endregion
        #region Overrides

        public override string ToString() 
        {
            return name;
        }

        #endregion
        #region Nested Classes

        /// <summary>
        /// Serializable class wrapper for an array of objects.
        /// </summary>
        [Serializable]
        public class ObjectArray
        {
            /// <summary>
            /// Max number of objects on each floor.
            /// </summary>
            const int MAX_OBJECTS_PER_FLOOR = 1024;

            /// <summary>
            /// Array of objects.
            /// </summary>
            [SerializeField]
            ObjectAttributes[] objects =
                new ObjectAttributes[MAX_OBJECTS_PER_FLOOR];

            /// <summary>
            /// Direct accessor.
            /// </summary>
            public ObjectAttributes this[int i]
            {
                get { return objects[i]; }
                set { objects[i] = value; }
            }

            /// <summary>
            /// Returns the maximum number of objects on this floor.
            /// </summary>
            public int Length { get { return MAX_OBJECTS_PER_FLOOR; } }

            /// <summary>
            /// Adds an object to this floor.
            /// This is O(n), so don't call this too frequently.
            /// </summary>
            public void Add(ObjectAttributes obj)
            {
                if (obj.Index == byte.MaxValue)
                    throw new IndexOutOfRangeException(
                        "Invalid index!" + obj.Index);

                // Look for an empty slot
                for (int i = 0; i < MAX_OBJECTS_PER_FLOOR; i++)
                    if (objects[i] == null ||
                        objects[i].Index == byte.MaxValue) {
                        objects[i] = obj;
                        return;
                    }

                throw new IndexOutOfRangeException("Too many objects!");
            }

            /// <summary>
            /// Removes the object at the given index.
            /// </summary>
            public void RemoveAt(int i)
            {
                objects[i] = null;
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
            SerializableStringPair[] attributes =
                new SerializableStringPair[MAX_OBJECT_ATTRIBUTES];

            /// <summary>
            /// Direct accessor.
            /// </summary>
            public SerializableStringPair this[int i]
            {
                get { return attributes[i]; }
                set { attributes[i] = value; }
            }

            /// <summary>
            /// Adds a special property to this object.
            /// </summary>
            public void Add(SerializableStringPair pair)
            {
                for (int i = 0; i < MAX_OBJECT_ATTRIBUTES; i++)
                    if (attributes[i] == null)
                    {
                        attributes[i] = pair;
                        return;
                    }
            }

            public override string ToString()
            {
                string result = "";
                for (int i = 0; i < MAX_OBJECT_ATTRIBUTES; i++)
                {
                    var attribute = attributes[i];
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
            [SerializeField] byte index = byte.MaxValue;
            [SerializeField] float posX;
            [SerializeField] float posY;
            [SerializeField] float posZ;
            [SerializeField] float rotX = 0f;
            [SerializeField] float rotY = 0f;
            [SerializeField] float rotZ = 0f;
            [SerializeField] float scaleX = 1f;
            [SerializeField] float scaleY = 1f;
            [SerializeField] float scaleZ = 1f;

            /// <summary>
            /// Special attributes.
            /// </summary>
            [SerializeField]
            AttributeArray attributes = new AttributeArray();

            /// <summary>
            /// Default constructor.
            /// !! Needed for proper serialization !!
            /// </summary>
            public ObjectAttributes()
            {
                index = byte.MaxValue;
            }

            public ObjectAttributes(byte index, Vector3 position, 
                float yRotation)
            {
                this.index = index;
                posX = position.x;
                posY = position.y;
                posZ = position.z;
                rotY = yRotation;
            }

            /// <summary>
            /// Returns the index of this object (read-only).
            /// </summary>
            public byte Index { get { return index; } }

            /// <summary>
            /// Returns the 3D position of this object (read-only).
            /// </summary>
            public Vector3 Position
            {
                get { return new Vector3(posX, posY, posZ); }
            }

            /// <summary>
            /// Returns the euler rotation of this object (read-only).
            /// </summary>
            public Vector3 EulerRotation
            {
                get { return new Vector3(rotX, rotY, rotZ); }
            }

            /// <summary>
            /// Gets/sets the y-rotation of this object.
            /// </summary>
            public float RotationY
            {
                get { return rotY; }
                set { rotY = value; }
            }

            /// <summary>
            /// Returns the 3D scale of this object (read-only).
            /// </summary>
            public Vector3 Scale
            {
                get {
                    return new Vector3(scaleX, scaleY, scaleZ);
                }
            }

            /// <summary>
            /// Gets/sets the x-scale of this object.
            /// </summary>
            public float ScaleX
            {
                get { return scaleX; }
                set { scaleX = value; }
            }

            /// <summary>
            /// Gets/sets the y-scale of this object.
            /// </summary>
            public float ScaleY
            {
                get { return scaleY; }
                set { scaleY = value; }
            }

            /// <summary>
            /// Gets/sets the z-scale of this object.
            /// </summary>
            public float ScaleZ
            {
                get { return scaleZ; }
                set { scaleZ = value; }
            }

            /// <summary>
            /// Returns the value of the attribute with the given name as a byte.
            /// </summary>
            public byte GetAttributeAsByte(string attribName)
            {
                for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++)
                {
                    var attribute = attributes[i];
                    if (attribute.Key == attribName)
                    {
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
            public float GetAttributeAsFloat(string attribName)
            {
                for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++)
                {
                    var attribute = attributes[i];
                    if (attribute.Key == attribName)
                    {
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
            public string GetAttributeAsString(string attribName)
            {
                for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++) {
                    var attribute = attributes[i];
                    if (attribute.Key == attribName)
                        return attribute.Value;
                }

                throw new NullReferenceException("Attribute \'" + attribName +
                    "\' not found!");
            }

            /// <summary>
            /// Sets the value of the attribute with the given name.
            /// </summary>
            public void SetAttribute(string attribName, string newValue)
            {
                for (int i = 0; i < AttributeArray.MAX_OBJECT_ATTRIBUTES; i++)
                {
                    var attribute = attributes[i];
                    if (attribute.Key == attribName)
                    {
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
            public void SetPosition(Vector3 pos)
            {
                posX = pos.x;
                posY = pos.y;
                posZ = pos.z;
            }

            /// <summary>
            /// Sets the rotation of an object with euler angles.
            /// </summary>
            public void SetEulerRotation(Vector3 euler)
            {
                rotX = euler.x;
                rotY = euler.y;
                rotZ = euler.z;
            }

            /// <summary>
            /// Sets the 3D local scale of an object.
            /// </summary>
            public void Set3DScale(Vector3 scale)
            {
                scaleX = scale.x;
                scaleY = scale.y;
                scaleZ = scale.z;
            }

            public override string ToString()
            {
                return attributes.ToString();
            }
        }

        #endregion
    }
}