// ChangeObjectSpecialAttributeAction.cs
// Author: Aaron

using System;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Action for changing special object attributes.
    /// </summary>
    [Serializable]
    public sealed class ChangeObjectSpecialAttributeAction : LevelEditorAction
    {
        #region Public Methods

        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override void Redo()
        {
            throw new NotImplementedException();
        }

        public override string ToShortString()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}