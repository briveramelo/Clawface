// ObjectAttributes.cs
// Author: Aaron

using System;

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Serializable class to represent object attributes.
    /// </summary>
    [Serializable]
    public sealed class ObjectAttributes
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Index of this object (used by ObjectDatabase).
        /// </summary>
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
        [SerializeField] Level.AttributeArray attributes
            = new Level.AttributeArray();

        #endregion
        #region Public Methods

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
            get
            {
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
            for (int i = 0; i < Level.AttributeArray.MAX_OBJECT_ATTRIBUTES; i++)
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
            for (int i = 0; i < Level.AttributeArray.MAX_OBJECT_ATTRIBUTES; i++)
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
            for (int i = 0; i < Level.AttributeArray.MAX_OBJECT_ATTRIBUTES; i++)
            {
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
            for (int i = 0; i < Level.AttributeArray.MAX_OBJECT_ATTRIBUTES; i++)
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

        #endregion
    }
}