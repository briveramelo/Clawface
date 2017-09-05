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
        #region Public Fields

        public const int LEVEL_WIDTH = 64;
        public const int LEVEL_DEPTH = 64;
        public const int LEVEL_HEIGHT = 10;

        #endregion
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Name of the level.
        /// </summary>
        [Tooltip("Name of the level.")]
        [SerializeField] string name;

        /// <summary>
        /// All objects in the level.
        /// </summary>
        [Tooltip("All objects in the level.")]
        [SerializeField]
        ObjectArray objects = new ObjectArray();

        #endregion
        #region Private Fields

        /// <summary>
        /// Default level name.
        /// </summary>
        const string DEFAULT_NAME = "New Level";

        #endregion
        #region Public Methods

        /// <summary>
        /// Default constructor -- inits object list.
        /// </summary>
        public Level()
        {
            name = DEFAULT_NAME;
            objects = new ObjectArray();
        }

        /// <summary>
        /// Gets/sets the name of this level.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

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

        public override string ToString() 
        {
            return name;
        }

        #endregion
        #region Public Structures

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

        #endregion
    }
}