// ChangeObjectNormalAttributeAction.cs
// Author: Aaron

using UnityEngine;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Action class for changing standard attributes (pos, rot, scale).
    /// </summary>
    public sealed class ChangeObjectNormalAttributeAction : LevelEditorAction
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// GameObject whose attribute was changed.
        /// </summary>
        [SerializeField]
        GameObject obj;

        /// <summary>
        /// Attribute that was changed.
        /// </summary>
        [SerializeField]
        AttributeChanged attrib;

        /// <summary>
        /// Old attribute value.
        /// </summary>
        [SerializeField]
        Vector3 oldValue;

        /// <summary>
        /// New attribute value.
        /// </summary>
        [SerializeField]
        Vector3 newValue;

        #endregion
        #region Public Structures

        /// <summary>
        /// Enum for the type of attribute that was changed.
        /// </summary>
        public enum AttributeChanged
        {
            Position,
            Rotation,
            Scale
        }

        #endregion
        #region Public Methods

        public ChangeObjectNormalAttributeAction(GameObject obj,
                AttributeChanged attrib, Vector3 newValue)
        {
            this.obj = obj;
            this.attrib = attrib;

            switch (attrib)
            {
                case AttributeChanged.Position:
                    oldValue = obj.transform.position;
                    break;

                case AttributeChanged.Rotation:
                    oldValue = obj.transform.rotation.eulerAngles;
                    break;

                case AttributeChanged.Scale:
                    oldValue = obj.transform.localScale;
                    break;
            }

            this.newValue = newValue;
        }

        public override void Undo()
        {
            switch (attrib)
            {
                case AttributeChanged.Position:
                    LevelManager.Instance.SetObjectPosition(obj,
                        oldValue, ActionType.Undo);
                    break;

                case AttributeChanged.Rotation:
                    LevelManager.Instance.SetObjectEulerRotation(obj,
                        oldValue, ActionType.Undo);
                    break;

                case AttributeChanged.Scale:
                    LevelManager.Instance.SetObject3DScale(obj,
                        oldValue, ActionType.Undo);
                    break;
            }

            LevelManager.Instance.RecordAttributeChange(obj,
                attrib, newValue, oldValue, ActionType.Undo);
        }

        public override void Redo()
        {
            switch (attrib)
            {
                case AttributeChanged.Position:
                    LevelManager.Instance.SetObjectPosition(obj,
                        newValue, ActionType.Redo);
                    break;

                case AttributeChanged.Rotation:
                    LevelManager.Instance.SetObjectEulerRotation(obj,
                        newValue, ActionType.Redo);
                    break;

                case AttributeChanged.Scale:
                    LevelManager.Instance.SetObject3DScale(obj,
                        newValue, ActionType.Redo);
                    break;
            }

            LevelManager.Instance.RecordAttributeChange(obj,
                attrib, oldValue, newValue, ActionType.Undo);
        }

        public override string ToShortString()
        {
            return string.Format("CHG {0} | Old: {1} | New: {2}",
                attrib.ToString().Substring(0, 1), oldValue.ToString(),
                newValue.ToString());
        }

        #endregion
    }
}