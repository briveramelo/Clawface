// LevelManagerConstants.cs
// Author: Aaron

namespace Turing.LevelEditor
{
    /// <summary>
    /// Holds constants useful to the LM.
    /// </summary>
    public partial class LevelManager : EditorSingleton<LevelManager>
    {
        #region Public Fields

        /// <summary>
        /// Scale value of a single tile.
        /// </summary>
        public const float TILE_UNIT_WIDTH = 5f;

        #endregion
        #region Private Fields

        /// <summary>
        /// Default level path.
        /// </summary>
        const string LEVEL_PATH = "Assets/Resources/Levels/";

        /// <summary>
        /// Editor name of the 3D asset preview object.
        /// </summary>
        const string PREVIEW_NAME = "~LevelEditorPreview";

        #endregion
    }
}